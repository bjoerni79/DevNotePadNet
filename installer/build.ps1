$source="installer.wxs"
$pathToWix="..\wix\bin"

function Run-Candle
{
    Write-Output "Calling candle..."
    #Invoke-Command -FilePath $exe -ArgumentList -v $source
    #..\wix\bin\candle.exe -v installer.wxs

    & $pathToWix\Candle.exe -v $source -o work\installer.wixobj 
}

function  Run-Light 
{
    Write-Output "Calling light..."

    & $pathToWix\light.exe -v work\installer.wixobj
}

function Prepare
{
    # Remove it if it already exists
    if (Test-Path work)
    {
        Remove-Item -force work 
    }

    New-Item -Name work -ItemType "directory"
}

## Main
Write-Output "Building the installer"
Write-Output "Path to Wix binaries : $pathToWix"
Write-Output "Source File : $source"



# Build the installer
Prepare
Run-Candle
Run-Light