#!/usr/bin/env pwsh
[CmdletBinding()]

param(
  [string]$Solution = "Tycho.slnx"
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$scriptDirectory = Split-Path -Path $MyInvocation.MyCommand.Path -Parent
$repoRoot = Split-Path -Path $scriptDirectory -Parent

Push-Location $repoRoot
try {
  $env:DOTNET_CLI_UI_LANGUAGE = "en-US"
  $env:DOTNET_SYSTEM_GLOBALIZATION_INVARIANT = "1"

  $solutionPath = Join-Path $repoRoot $Solution
  dotnet format $solutionPath --severity info --verbosity diagnostic
}
finally {
  $env:DOTNET_CLI_UI_LANGUAGE = $null
  $env:DOTNET_SYSTEM_GLOBALIZATION_INVARIANT = $null
  Pop-Location
}
