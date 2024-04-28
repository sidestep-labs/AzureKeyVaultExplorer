

Push-Location ./kvexplorer.Desktop/mac


$filesToMove = Get-ChildItem  -Exclude @("*.pdb","*.dsym")

$initialRootDir = "Key Vault Explorer"

if (-not (Test-Path $initialRootDir)) {
    New-Item -ItemType Directory -Path $initialRootDir -Force | Out-Null
}

$contentsDir = "$initialRootDir\Contents"
if (-not (Test-Path $contentsDir)) {
    New-Item -ItemType Directory -Path $contentsDir -Force | Out-Null
}

$macOSPath = "$($initialRootDir)\$contentsDir\MacOS"
if (-not (Test-Path $macOSPath)) {
    New-Item -ItemType Directory -Path $macOSPath -Force | Out-Null
}
foreach($file in $filesToMove){
    Copy-Item -Path $file.FullName -Destination $macOSPath -Force
}


$resourcesPath = "$($initialRootDir)\Contents\Resources"
if (-not (Test-Path $resourcesPath)) {
    New-Item -ItemType Directory -Path $resourcesPath -Force | Out-Null
}
#Create Info.plist file under Contents
# $infoPath = Join-Path (Join-Path $initialRootDir "Contents") "Info.plist"
# New-Item -ItemType File -Path $infoPath | Out-Null




Rename-Item -Path $initialRootDir -NewName "$($initialRootDir).app" -Force

Pop-Location