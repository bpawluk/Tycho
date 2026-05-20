#!/usr/bin/env pwsh
[CmdletBinding()]

param(
  [string]$SourceCode = ".",
  [string]$OptionsJson = "devskim-options.json",
  [string]$ReportDirectory = "Artifacts/SecurityAnalysis",
  [string]$OutputFileName = "security-analysis-report.sarif"
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$env:DOTNET_SYSTEM_GLOBALIZATION_INVARIANT = "1"

$scriptDirectory = Split-Path -Path $MyInvocation.MyCommand.Path -Parent
$repoRoot = Split-Path -Path $scriptDirectory -Parent

Push-Location $repoRoot
try {
  $sourcePath = Join-Path $repoRoot $SourceCode
  $optionsPath = Join-Path $repoRoot $OptionsJson
  $manifestPath = Join-Path $repoRoot "dotnet-tools.json"
  $reportPath = Join-Path $repoRoot $ReportDirectory
  $outputPath = Join-Path $reportPath $OutputFileName

  if (-not (Test-Path $sourcePath)) {
    throw "Source path not found: $sourcePath"
  }

  if (-not (Test-Path $optionsPath)) {
    throw "DevSkim options file not found: $optionsPath"
  }

  if (-not (Test-Path $manifestPath)) {
    throw "Tool manifest not found: $manifestPath"
  }

  if (Test-Path $reportPath) {
    Remove-Item -Path $reportPath -Recurse -Force
  }

  New-Item -Path $reportPath -ItemType Directory -Force | Out-Null

  Write-Host "Restoring local .NET tools from $manifestPath"
  dotnet tool restore --tool-manifest $manifestPath
  if ($LASTEXITCODE -ne 0) {
    throw "Restoring local .NET tools failed with exit code $LASTEXITCODE"
  }

  Write-Host ""
  Write-Host "Running DevSkim security analysis"
  dotnet tool run devskim analyze --source-code $sourcePath --output-file $outputPath --options-json $optionsPath
  if ($LASTEXITCODE -ne 0) {
    throw "DevSkim analysis command failed with exit code $LASTEXITCODE"
  }

  if (-not (Test-Path $outputPath)) {
    throw "DevSkim did not produce a SARIF report: $outputPath"
  }

  $sarif = Get-Content -Path $outputPath -Raw | ConvertFrom-Json

  $warningCount = 0
  $errorCount = 0

  foreach ($run in @($sarif.runs)) {
    $defaultLevelsByRuleId = @{}
    foreach ($rule in @($run.tool.driver.rules)) {
      $ruleIdProperty = $rule.PSObject.Properties["id"]
      if ($null -eq $ruleIdProperty -or [string]::IsNullOrWhiteSpace([string]$ruleIdProperty.Value)) {
        continue
      }

      $defaultConfigurationProperty = $rule.PSObject.Properties["defaultConfiguration"]
      if ($null -eq $defaultConfigurationProperty -or $null -eq $defaultConfigurationProperty.Value) {
        continue
      }

      $defaultLevelProperty = $defaultConfigurationProperty.Value.PSObject.Properties["level"]
      if ($null -eq $defaultLevelProperty -or [string]::IsNullOrWhiteSpace([string]$defaultLevelProperty.Value)) {
        continue
      }

      $defaultLevelsByRuleId[[string]$ruleIdProperty.Value] = [string]$defaultLevelProperty.Value
    }

    foreach ($result in @($run.results)) {
      $level = $null

      $levelProperty = $result.PSObject.Properties["level"]
      if ($null -ne $levelProperty -and -not [string]::IsNullOrWhiteSpace([string]$levelProperty.Value)) {
        $level = [string]$levelProperty.Value
      }
      else {
        $ruleIdProperty = $result.PSObject.Properties["ruleId"]
        if ($null -ne $ruleIdProperty -and $defaultLevelsByRuleId.ContainsKey([string]$ruleIdProperty.Value)) {
          $level = $defaultLevelsByRuleId[[string]$ruleIdProperty.Value]
        }
      }

      if ([string]::Equals($level, "warning", [System.StringComparison]::OrdinalIgnoreCase)) {
        $warningCount += 1
      }
      elseif ([string]::Equals($level, "error", [System.StringComparison]::OrdinalIgnoreCase)) {
        $errorCount += 1
      }
    }
  }

  Write-Host ""
  Write-Host "DevSkim SARIF report generated: $outputPath"

  if (($warningCount + $errorCount) -gt 0) {
    Write-Host "Discovered $errorCount error(s) and $warningCount warning(s)"
    Write-Host ""
    throw "This repository's source code contains potential security issues that should be reviewed and addressed"
  }
}
finally {
  Pop-Location
}