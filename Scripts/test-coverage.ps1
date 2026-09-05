#!/usr/bin/env pwsh
[CmdletBinding()]

param(
  [string]$Solution = "Tycho.slnx",
  [string]$CoverageSettings = "coverage.runsettings",
  [string]$ResultsDirectory = "Artifacts/TestCoverage/Results",
  [string]$ReportDirectory = "Artifacts/TestCoverage/Report",
  [string]$ReportTypes = "Html;HtmlSummary;Cobertura"
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$env:DOTNET_SYSTEM_GLOBALIZATION_INVARIANT="1"

$scriptDirectory = Split-Path -Path $MyInvocation.MyCommand.Path -Parent
$repoRoot = Split-Path -Path $scriptDirectory -Parent

Push-Location $repoRoot
try {
  $solutionPath = Join-Path $repoRoot $Solution
  $settingsPath = Join-Path $repoRoot $CoverageSettings
  $manifestPath = Join-Path $repoRoot "dotnet-tools.json"
  $resultsPath = Join-Path $repoRoot $ResultsDirectory
  $reportPath = Join-Path $repoRoot $ReportDirectory

  if (-not (Test-Path $solutionPath)) {
    throw "Solution file not found: $solutionPath"
  }

  if (-not (Test-Path $settingsPath)) {
    throw "Coverage settings file not found: $settingsPath"
  }

  if (-not (Test-Path $manifestPath)) {
    throw "Tool manifest not found: $manifestPath"
  }

  if (Test-Path $resultsPath) {
    Remove-Item -Path $resultsPath -Recurse -Force
  }

  if (Test-Path $reportPath) {
    Remove-Item -Path $reportPath -Recurse -Force
  }

  Write-Host "Restoring local .NET tools from $manifestPath"
  dotnet tool restore --tool-manifest $manifestPath
  if ($LASTEXITCODE -ne 0) {
    throw "Restoring local .NET tools failed with exit code $LASTEXITCODE"
  }

  Write-Host ""
  Write-Host "Running tests with XPlat Code Coverage"
  dotnet test $solutionPath --settings $settingsPath --collect:"XPlat Code Coverage" --results-directory $resultsPath
  if ($LASTEXITCODE -ne 0) {
    throw "Running tests failed with exit code $LASTEXITCODE"
  }

  $coverageFiles = @(Get-ChildItem -Path $resultsPath -Filter "coverage.cobertura.xml" -Recurse -File | Select-Object -ExpandProperty FullName)
  if ($coverageFiles.Count -eq 0) {
    throw "No coverage.cobertura.xml files were generated under $resultsPath"
  }

  $reportsArgument = [string]::Join(";", $coverageFiles)

  Write-Host ""
  Write-Host "Merging coverage results and generating report"
  dotnet tool run reportgenerator "-reports:$reportsArgument" "-targetdir:$reportPath" "-reporttypes:$ReportTypes"
  if ($LASTEXITCODE -ne 0) {
    throw "Generating report failed with exit code $LASTEXITCODE"
  }

  Write-Host ""
  Write-Host "Coverage report generated successfully: $reportPath"
}
finally {
  Pop-Location
}