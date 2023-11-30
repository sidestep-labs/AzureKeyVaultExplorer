param(
    [switch]$RunBuild = $false,
    [System.Diagnostics.Stopwatch]$sw = [System.Diagnostics.Stopwatch]::StartNew(),
    [string]$BuildNumber = '0.0.1.2',
    $VersionPrefix = "1.0.0",
    $VersionSuffix = "99",
    [ValidateSet('net8.0', 'net8.0-windows10.0.19041.0')]
    [string]$Platform = 'net8.0'
)
$DebugPreference = 'continue';
# https://github.com/AvaloniaUI/Avalonia/issues/9503
if ($RunBuild) {
    Push-Location  C:\repos\sidestep\kvexplorer.Desktop;
    $env:KVEXPLORER_APP_VERSION = $BuildNumber
    dotnet publish  -o publish/ -c Release --self-contained -p:VersionPrefix=$VersionPrefix -p:VersionSuffix=$VersionSuffix -f $Platform
    #New-Item -Path $ProjectDir -Name "VERSION" -ItemType "file" -Value $BuildNumber -Force
    explorer.exe .
    pop-location
    $sw.Stop()
    $sw
    Push-Location  c:\repos\sidestep\kvexplorer.Desktop/publish
    .\KeyVaultExplorer.exe 
    return;
}
Pop-Location
$sw.Stop()
Write-Debug "App elapsed start up: $($sw.Elapsed)"
$sw

