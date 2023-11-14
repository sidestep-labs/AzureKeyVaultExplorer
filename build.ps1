param(
    [switch]$RunBuild = $false,
    [System.Diagnostics.Stopwatch]$sw = [System.Diagnostics.Stopwatch]::StartNew(),
    [string]$BuildNumber = '1.0.0.0',
    $VersionPrefix = "1.0.0",
    $VersionSuffix = "99"
)
$DebugPreference = 'continue';
# https://github.com/AvaloniaUI/Avalonia/issues/9503
if ($RunBuild) {
    Push-Location  c:\repos\sidestep\avalonia.kvexplorer.Desktop;

    $env:KVEXPLORER_APP_VERSION = $BuildNumber
    dotnet publish  -o publish/ -c Release --self-contained -p:VersionPrefix=$VersionPrefix -p:VersionSuffix=$VersionSuffix
       #New-Item -Path $ProjectDir -Name "VERSION" -ItemType "file" -Value $BuildNumber -Force

    explorer.exe .
    
    pop-location
    $sw.Stop()

    $sw
  
    return;
}

Push-Location  c:\repos\sidestep\avalonia.kvexplorer.Desktop/publish

.\KeyVaultExplorer.exe 

Pop-Location
$sw.Stop()

Write-Debug "App elapsed start up: $($sw.Elapsed)"

$sw

