param(
    [string]$ModId = "FullDeckPrune",
    [string]$Configuration = "Release",
    [string]$OutputRoot = "dist"
)

$ErrorActionPreference = "Stop"

$repoRoot = Resolve-Path (Join-Path $PSScriptRoot "..")
$outputRootPath = Join-Path $repoRoot $OutputRoot
$packageRoot = Join-Path $outputRootPath $ModId
$zipPath = Join-Path $outputRootPath "$ModId.zip"

$manifestPath = Join-Path $repoRoot "$ModId.json"
$projectManifestPath = Join-Path $repoRoot "$ModId\$ModId.json"
$dllCandidates = @(
    (Join-Path $repoRoot "$ModId\.godot\mono\temp\bin\$Configuration\$ModId.dll"),
    (Join-Path $repoRoot "$ModId\bin\$Configuration\net9.0\$ModId.dll"),
    (Join-Path $repoRoot "bin\$Configuration\net9.0\$ModId.dll"),
    (Join-Path $repoRoot "$ModId.dll")
)
$pckCandidates = @(
    (Join-Path $repoRoot "$ModId\.godot\mono\temp\bin\$Configuration\$ModId.pck"),
    (Join-Path $repoRoot "$ModId\bin\$Configuration\net9.0\$ModId.pck"),
    (Join-Path $repoRoot "bin\$Configuration\net9.0\$ModId.pck"),
    (Join-Path $repoRoot "$ModId.pck")
)

if (Test-Path $projectManifestPath) {
    $manifestPath = $projectManifestPath
}

$dllPath = $dllCandidates | Where-Object { Test-Path $_ } | Select-Object -First 1
$pckPath = $pckCandidates | Where-Object { Test-Path $_ } | Select-Object -First 1

if (-not (Test-Path $manifestPath)) {
    throw "Missing manifest: $manifestPath"
}

if (-not $dllPath) {
    throw "Missing $ModId.dll. Build the mod before packaging."
}

New-Item -ItemType Directory -Force -Path $packageRoot | Out-Null
Copy-Item -Force $manifestPath (Join-Path $packageRoot "$ModId.json")
Copy-Item -Force $dllPath (Join-Path $packageRoot "$ModId.dll")

if ($pckPath) {
    Copy-Item -Force $pckPath (Join-Path $packageRoot "$ModId.pck")
} else {
    Write-Warning "No $ModId.pck found. This is okay only if the manifest has has_pck=false."
}

if (Test-Path $zipPath) {
    Remove-Item -Force $zipPath
}

Compress-Archive -Path $packageRoot -DestinationPath $zipPath -Force
Write-Host "Created $zipPath"
