#!/usr/bin/env pwsh
[CmdletBinding()]
param(
  [ValidateSet("All", "Tycho", "Persistence", "SourceGenerator")]
  [string]$Target = "All",
  [string]$ReportDirectory = "Artifacts/MutationTesting",
  [ValidateRange(1, 128)]
  [int]$Concurrency,
  [string[]]$Mutate
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$scriptDirectory = Split-Path -Path $MyInvocation.MyCommand.Path -Parent
$repoRoot = Split-Path -Path $scriptDirectory -Parent
$manifestPath = Join-Path $repoRoot "dotnet-tools.json"
$artifactsPath = [IO.Path]::GetFullPath((Join-Path $repoRoot "Artifacts"))
$reportPath = if ([IO.Path]::IsPathRooted($ReportDirectory)) {
  [IO.Path]::GetFullPath($ReportDirectory)
} else {
  [IO.Path]::GetFullPath((Join-Path $repoRoot $ReportDirectory))
}
$pathComparison = if ([Environment]::OSVersion.Platform -eq [PlatformID]::Win32NT) { [StringComparison]::OrdinalIgnoreCase } else { [StringComparison]::Ordinal }
if (-not $reportPath.StartsWith($artifactsPath + [IO.Path]::DirectorySeparatorChar, $pathComparison)) {
  throw "ReportDirectory must be a subdirectory of $artifactsPath"
}

$projects = [ordered]@{
  Tycho = "Tycho"
  Persistence = "Tycho.Persistence.EFCore"
  SourceGenerator = "Tycho.Utils.SourceGenerator"
}
$targets = if ($Target -eq "All") { @($projects.Keys) } else { @($projects.Keys | Where-Object { $_ -eq $Target }) }
foreach ($name in $targets) {
  $configPath = Join-Path $scriptDirectory "MutationTesting/$name.json"
  if (-not (Test-Path -LiteralPath $configPath -PathType Leaf)) {
    throw "Mutation settings file not found: $configPath"
  }
  $projectPath = Join-Path $repoRoot "$($projects[$name])/$($projects[$name]).csproj"
  if (-not (Test-Path -LiteralPath $projectPath -PathType Leaf)) {
    throw "Project file not found: $projectPath"
  }
}
if (-not (Test-Path -LiteralPath $manifestPath -PathType Leaf)) {
  throw "Tool manifest not found: $manifestPath"
}

$previousLanguage = $env:DOTNET_CLI_UI_LANGUAGE
$env:DOTNET_CLI_UI_LANGUAGE = "en-US"
Push-Location $repoRoot
try {
  Write-Host "Restoring local .NET tools from $manifestPath"
  dotnet tool restore --tool-manifest $manifestPath
  if ($LASTEXITCODE -ne 0) {
    throw "Restoring local .NET tools failed with exit code $LASTEXITCODE"
  }

  foreach ($name in $targets) {
    $targetReportPath = Join-Path $reportPath $name
    # Only remove the selected target's output. Reject links before recursive cleanup.
    $ancestor = $targetReportPath
    while ($ancestor -and $ancestor -ne $repoRoot) {
      if (Test-Path -LiteralPath $ancestor) {
        if ((Get-Item -LiteralPath $ancestor -Force).Attributes -band [IO.FileAttributes]::ReparsePoint) {
          throw "Mutation output path cannot contain a symbolic link or junction: $ancestor"
        }
      }
      $ancestor = Split-Path -Path $ancestor -Parent
    }
    if (Test-Path -LiteralPath $targetReportPath) {
      Remove-Item -LiteralPath $targetReportPath -Recurse -Force
    }

    $strykerArguments = @("--config-file", (Join-Path $scriptDirectory "MutationTesting/$name.json"),
      "--output", $targetReportPath, "--skip-version-check")
    if ($PSBoundParameters.ContainsKey("Concurrency")) {
      $strykerArguments += @("--concurrency", "$Concurrency")
    }
    if ($PSBoundParameters.ContainsKey("Mutate")) {
      # CLI include patterns replace the config's patterns, so preserve its exclusions.
      $config = Get-Content -LiteralPath (Join-Path $scriptDirectory "MutationTesting/$name.json") -Raw | ConvertFrom-Json
      foreach ($pattern in @($Mutate) + @($config.'stryker-config'.mutate | Where-Object { $_.StartsWith("!") })) {
        if ([string]::IsNullOrWhiteSpace($pattern)) { throw "Mutation patterns cannot be empty" }
        $strykerArguments += @("--mutate", $pattern)
      }
    }

    Write-Host "Running mutation tests for $name"
    Push-Location (Join-Path $repoRoot $projects[$name])
    try {
      dotnet tool run dotnet-stryker -- @strykerArguments
      if ($LASTEXITCODE -ne 0) {
        throw "Mutation testing for $name failed with exit code $LASTEXITCODE. Output: $targetReportPath"
      }
    }
    finally { Pop-Location }

    foreach ($extension in @("html", "json")) {
      $reports = @(Get-ChildItem -LiteralPath $targetReportPath -Filter "mutation-report.$extension" -Recurse -File)
      if ($reports.Count -eq 0) { throw "No $extension mutation report generated for $name under $targetReportPath" }
      foreach ($report in $reports) { Write-Host "Mutation report: $($report.FullName)" }
    }
  }
}
finally {
  Pop-Location
  $env:DOTNET_CLI_UI_LANGUAGE = $previousLanguage
}
