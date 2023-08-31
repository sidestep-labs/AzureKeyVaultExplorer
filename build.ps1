param(
    [switch]$RunBuild = $false,
    [System.Diagnostics.Stopwatch]$sw = [System.Diagnostics.Stopwatch]::StartNew()
)
$DebugPreference = 'continue';

if ($RunBuild) {
    Push-Location  c:\repos\sidestep\avalonia.kvexplorer.Desktop
    dotnet publish -o publish/ -c Release --self-contained
    Push-Location  c:\repos\sidestep\avalonia.kvexplorer.Desktop/publish
    explorer.exe .
    $sw.Stop()

    $sw
    Pop-Location
    return;
}

Push-Location  c:\repos\sidestep\avalonia.kvexplorer.Desktop/publish

.\avalonia.kvexplorer.Desktop.exe 

Pop-Location
$sw.Stop()

Write-Debug "App elapsed start up: $($sw.Elapsed)"

$sw

