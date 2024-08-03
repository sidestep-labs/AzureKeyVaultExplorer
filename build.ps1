param(
    [switch]$PublishAot = $true,
    [string]$BuildNumber = '1.0.0.0',
    $VersionPrefix = "1.0.0",
    $VersionSuffix = "99",
    [ValidateSet('net8.0', 'net8.0-windows10.0.19041.0', "net8.0-macos")]
    [string]$Platform = 'net8.0',
    [ValidateSet('win-x64', 'win-arm64', 'osx-x64', 'osx-arm64')]
    [string]$Runtime = 'win-x64',
    [Switch]$CreateMacOSBundle = $false
)
$DebugPreference = 'continue';
# https://github.com/AvaloniaUI/Avalonia/issues/9503

Set-Content -Path "VERSION" -Value $BuildNumber -Force

$command = @"
dotnet publish  ./Desktop/Desktop.csproj `
    -r $Runtime `
    -o .\publish `
    -c Release  `
    -f $Platform `
    -p:PublishAot=$PublishAot `
    -p:PublishReadyToRun=true  `
    -p:PublishTrimmed=$PublishAot `
    -p:TrimMode=link `
    -p:IncludeNativeLibrariesForSelfExtract=true `
    -p:PublishSingleFile=$($PublishAot ? "false":"true") `
    --self-contained 
"@

Write-Host $command -ForegroundColor Green


dotnet publish  ./Desktop/Desktop.csproj `
    -r $Runtime `
    -o .\publish `
    -c Release  `
    -f $Platform `
    -p:PublishAot=$PublishAot `
    -p:PublishReadyToRun=true  `
    -p:PublishTrimmed=$PublishAot `
    -p:TrimMode=link `
    -p:IncludeNativeLibrariesForSelfExtract=true `
    -p:PublishSingleFile=$($PublishAot ? "false":"true") `
    --self-contained 
    # -p:UseAppHost=true `



if ($Runtime -eq "osx-x64") { 

    $initialRootDir = "macOSBundledFolder"
    $contentsDir = "$initialRootDir\Contents"
    $macOSDir = "$($initialRootDir)\Contents\MacOS"
    $resourcesPath = "$($initialRootDir)\Contents\Resources"

    New-Item -ItemType Directory -Path $initialRootDir -Force | Out-Null
    New-Item -ItemType Directory -Path $contentsDir -Force | Out-Null
    New-Item -ItemType Directory -Path $macOSDir -Force | Out-Null
    New-Item -ItemType Directory -Path $resourcesPath -Force | Out-Null

    $filesToMove = Get-ChildItem  -Exclude @("*.pdb", "*.dsym", "Key Vault Explorer for Azure")  -Path .\publish
    foreach ($file in $filesToMove) {
        Copy-Item -Path $file -Destination $macOSDir -Force 
    }
    Copy-Item -Path ".\KeyVaultExplorer\Assets\Info.plist" -Destination $contentsDir -Force
    Copy-Item -Path ".\KeyVaultExplorer\Assets\AppIcon.icns" -Destination $resourcesPath -Force

    # $filesToModify = Get-ChildItem  -Path $macOSDir 
    # foreach ($file in $filesToModify) {
    #     chmod +x $file 
    # }
    Rename-Item -Path $initialRootDir -NewName "Key Vault Explorer for Azure.app" -Force 

}



# //TODO create a script that can edit the appxmanifest to change settings and repack the app for msft store submission
# mpdev build .\msix.json
# Push-Location 'C:\Program Files (x86)\Windows Kits\10\bin\10.0.19041.0\x64\'
# .\makeappx.exe unpack /v /p "C:\repos\AzureKeyVaultExplorer\mpdev\Key Vault Explorer\output\Key Vault Explorer 1.0.263.0.msix" /d "C:\repos\AzureKeyVaultExplorer\mpdev\Key Vault Explorer\output\Unpacked"
# .\makeappx.exe pack /v /d "C:\repos\AzureKeyVaultExplorer\mpdev\Key Vault Explorer\output\Unpacked" /p "C:\repos\AzureKeyVaultExplorer\mpdev\Key Vault Explorer\output\KeyVaultExplorerforAzure_1263_1.msix"

