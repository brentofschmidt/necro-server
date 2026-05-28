# Copy Game.Core build output into Unity's Plugins/ folder.
# Run after `dotnet build Game.Core -c Release`.

$ErrorActionPreference = 'Stop'

$sourceDir = Join-Path $PSScriptRoot '..\Game.Core\bin\Release\netstandard2.1'
$targetDir = 'C:\Users\brent\UnityProjects\Necro\Assets\Plugins'

if (-not (Test-Path $sourceDir)) {
    throw "Source not found: $sourceDir`nRun 'dotnet build Game.Core -c Release' first."
}
if (-not (Test-Path $targetDir)) {
    throw "Target not found: $targetDir"
}

$dlls = Get-ChildItem -Path $sourceDir -Filter '*.dll'
if ($dlls.Count -eq 0) {
    throw "No DLLs in $sourceDir"
}

foreach ($dll in $dlls) {
    Copy-Item -Path $dll.FullName -Destination $targetDir -Force
    Write-Host "  $($dll.Name)"
}

Write-Host ""
Write-Host "Copied $($dlls.Count) DLL(s) to $targetDir"
