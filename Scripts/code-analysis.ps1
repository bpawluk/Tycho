#!/usr/bin/env pwsh
[CmdletBinding()]

param(
  [string]$Solution = "Tycho.slnx",
  [string]$ReportDirectory = "Artifacts/CodeAnalysis"
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$env:DOTNET_SYSTEM_GLOBALIZATION_INVARIANT="1"

$scriptDirectory = Split-Path -Path $MyInvocation.MyCommand.Path -Parent
$repoRoot = Split-Path -Path $scriptDirectory -Parent

Push-Location $repoRoot
try {
  $solutionPath = Join-Path $repoRoot $Solution
  $reportPath = Join-Path $repoRoot $ReportDirectory

  if (Test-Path $reportPath) {
    Remove-Item -Path $reportPath -Recurse -Force
  }

  dotnet format $solutionPath --severity info --verbosity diagnostic --verify-no-changes --report $reportPath
}
finally {
  Pop-Location
}