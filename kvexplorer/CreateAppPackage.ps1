Push-Location ./kvexplorer.Desktop/mac

$initialRootDir = "Key Vault Explorer"
$contentsDir = "$initialRootDir\Contents"
$macOSDir = "$($initialRootDir)\Contents\MacOS"
$resourcesPath = "$($initialRootDir)\Contents\Resources"

New-Item -ItemType Directory -Path $initialRootDir -Force | Out-Null
New-Item -ItemType Directory -Path $contentsDir -Force | Out-Null
New-Item -ItemType Directory -Path $macOSDir -Force | Out-Null
New-Item -ItemType Directory -Path $resourcesPath -Force | Out-Null


$filesToMove = Get-ChildItem  -Exclude @("*.pdb", "*.dsym","Key Vault Explorer") 
foreach ($file in $filesToMove) {
    Copy-Item -Path $file -Destination $macOSDir -Force
}

Copy-Item -Path "../../kvexplorer/Assets/Info.plist" -Destination $contentsDir -Force
# Copy-Item -Path "/kvexplorer/Assets/AppSettings.plist" -Destination $resourcesPath -Force
Copy-Item -Path "../../kvexplorer/Assets/AppIcon.icns" -Destination $resourcesPath -Force

Rename-Item -Path $initialRootDir -NewName "$($initialRootDir).app" -Force 

Pop-Location