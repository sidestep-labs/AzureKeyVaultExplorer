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
    Push-Location  C:\repos\kvexplorer\kvexplorer.Desktop;
    $env:KVEXPLORER_APP_VERSION = $BuildNumber
    dotnet publish  -o publish/ -c Release --self-contained -p:VersionPrefix=$VersionPrefix -p:VersionSuffix=$VersionSuffix -f $Platform -p:PublishAot=true  -p:PublishReadyToRun=true 
    dotnet publish -c Release -o ..\publish\ -p:PublishSingleFile=true -p:PublishTrimmed=true -p:TrimMode=link -p:IncludeNativeLibrariesForSelfExtract=true --self-contained=true -p:VersionPrefix=$VersionPrefix -p:VersionSuffix=$VersionSuffix -f $Platform  -p:PublishReadyToRun=true 
    #New-Item -Path $ProjectDir -Name "VERSION" -ItemType "file" -Value $BuildNumber -Force
    explorer.exe .
    pop-location
    $sw.Stop()
    $sw
    Push-Location  c:\repos\kvexplorer\kvexplorer.Desktop/publish
    Pop-Location
    return;
}
Push-Location  c:\repos\kvexplorer\kvexplorer.Desktop/publish
Pop-Location
$sw.Stop()
Write-Debug "App elapsed start up: $($sw.Elapsed)"
$sw

