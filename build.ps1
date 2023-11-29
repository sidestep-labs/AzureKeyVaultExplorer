param(
    [switch]$RunBuild = $false,
    [switch]$RunBuildWindows = $false,
    [System.Diagnostics.Stopwatch]$sw = [System.Diagnostics.Stopwatch]::StartNew(),
    [string]$BuildNumber = '1.0.0.0',
    $VersionPrefix = "1.0.0",
    $VersionSuffix = "99"
)
$DebugPreference = 'continue';
# https://github.com/AvaloniaUI/Avalonia/issues/9503
if ($RunBuild) {
    
    Push-Location  C:\repos\sidestep\kvexplorer.Desktop;

    $env:KVEXPLORER_APP_VERSION = $BuildNumber
    dotnet publish  -o publish/ -c Release --self-contained -p:VersionPrefix=$VersionPrefix -p:VersionSuffix=$VersionSuffix -f net8.0
    #New-Item -Path $ProjectDir -Name "VERSION" -ItemType "file" -Value $BuildNumber -Force

    explorer.exe .
    
    pop-location
    $sw.Stop()

    $sw
  
    Push-Location  c:\repos\sidestep\kvexplorer.Desktop/publish

    .\KeyVaultExplorer.exe 

    return;
}

if ($RunBuildWindows) {
    
    Push-Location  C:\repos\sidestep\kvexplorer.Desktop;

    $env:KVEXPLORER_APP_VERSION = $BuildNumber
    dotnet publish  -o publishWin/ -c Release --self-contained -p:VersionPrefix=$VersionPrefix -p:VersionSuffix=$VersionSuffix -f net8.0-windows10.0.19041.0 --publish-aot
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

