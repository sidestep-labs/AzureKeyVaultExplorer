param(
    [switch]$RunBuild = $false,
    [string]$BuildNumber = '0.0.1.0',
    $VersionPrefix = "1.0.0",
    $VersionSuffix = "99",
    [ValidateSet('net8.0', 'net8.0-windows10.0.19041.0')]
    [string]$Platform = 'net8.0',
    [ValidateSet('win-x64', 'win-arm64', 'osx-x64', 'osx-arm64')]
    [string]$Runtime = 'win-arm64'
)
$DebugPreference = 'continue';
# https://github.com/AvaloniaUI/Avalonia/issues/9503
if ($RunBuild) {
    Push-Location  .\kvexplorer.Desktop;
    $env:KVEXPLORER_APP_VERSION = $BuildNumber
    # // -p:PublishSingleFile=true
    # --self-contained
    dotnet publish  --runtime $Runtime  -o publish/ -c Release  -p:VersionPrefix=$VersionPrefix -p:VersionSuffix=$VersionSuffix -f $Platform -p:PublishAot=true  -p:PublishReadyToRun=true  -p:PublishTrimmed=true -p:TrimMode=partial -p:IncludeNativeLibrariesForSelfExtract=true 
    Pop-Location
}

if ($Runtime -like "osx") { 
    Push-Location  .\KeyVaultExplorer\kvexplorer.Desktop\publish

    $initialRootDir = "Key Vault Explorer"
    $contentsDir = "$initialRootDir\Contents"
    $macOSDir = "$($initialRootDir)\Contents\MacOS"
    $resourcesPath = "$($initialRootDir)\Contents\Resources"

    New-Item -ItemType Directory -Path $initialRootDir -Force | Out-Null
    New-Item -ItemType Directory -Path $contentsDir -Force | Out-Null
    New-Item -ItemType Directory -Path $macOSDir -Force | Out-Null
    New-Item -ItemType Directory -Path $resourcesPath -Force | Out-Null


    $filesToMove = Get-ChildItem  -Exclude @("*.pdb", "*.dsym", "Key Vault Explorer") 
    foreach ($file in $filesToMove) {
        Copy-Item -Path $file -Destination $macOSDir -Force
    }

    Copy-Item -Path "../../kvexplorer/Assets/Info.plist" -Destination $contentsDir -Force
    Copy-Item -Path "../../kvexplorer/Assets/AppIcon.icns" -Destination $resourcesPath -Force

    Rename-Item -Path $initialRootDir -NewName "$($initialRootDir).app" -Force 

    Pop-Location
}