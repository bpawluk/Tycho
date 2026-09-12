#!/usr/bin/env pwsh
[CmdletBinding()]

param(
  [string]$Solution = "Tycho.slnx"
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$env:DOTNET_SYSTEM_GLOBALIZATION_INVARIANT="1"

$scriptDirectory = Split-Path -Path $MyInvocation.MyCommand.Path -Parent
$repoRoot = Split-Path -Path $scriptDirectory -Parent

Push-Location $repoRoot
try {
  $solutionPath = Join-Path $repoRoot $Solution
  dotnet format $solutionPath --severity info --verbosity diagnostic
}
finally {
  Pop-Location
}