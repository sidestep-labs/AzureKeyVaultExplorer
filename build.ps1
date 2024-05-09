param(
    [switch]$PublishAot = $false,
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
$env:KVEXPLORER_APP_VERSION = $BuildNumber
dotnet publish  ./KeyVaultExplorer.Desktop/KeyVaultExplorer.Desktop.csproj `
    --runtime $Runtime `
    -o .\publish `
    -c Release  `
    -p:VersionPrefix=$VersionPrefix `
    -p:VersionSuffix=$VersionSuffix `
    -f $Platform `
    -p:PublishAot=$PublishAot  `
    -p:PublishReadyToRun=true  `
    -p:PublishTrimmed=true `
    -p:TrimMode=partial `
    -p:IncludeNativeLibrariesForSelfExtract=true `
    -p:PublishSingleFile=$($PublishAot ? "false":"true") `
    -p:UseAppHost=true `
    --self-contained 

if ($Runtime -match "osx") { 

    $initialRootDir = "macOSBundledFolder"
    $contentsDir = "$initialRootDir\Contents"
    $macOSDir = "$($initialRootDir)\Contents\MacOS"
    $resourcesPath = "$($initialRootDir)\Contents\Resources"

    New-Item -ItemType Directory -Path $initialRootDir -Force | Out-Null
    New-Item -ItemType Directory -Path $contentsDir -Force | Out-Null
    New-Item -ItemType Directory -Path $macOSDir -Force | Out-Null
    New-Item -ItemType Directory -Path $resourcesPath -Force | Out-Null

    $filesToMove = Get-ChildItem  -Exclude @("*.pdb", "*.dsym", "Key Vault Explorer")  -Path .\publish
    foreach ($file in $filesToMove) {
        Copy-Item -Path $file -Destination $macOSDir -Force 
    }
    Copy-Item -Path ".\KeyVaultExplorer\Assets\Info.plist" -Destination $contentsDir -Force
    Copy-Item -Path ".\KeyVaultExplorer\Assets\AppIcon.icns" -Destination $resourcesPath -Force
    Rename-Item -Path $initialRootDir -NewName "Key Vault Explorer.app" -Force 
}
