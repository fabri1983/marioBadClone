# A PowerShell script to automate the installation of the specified Unity in a standard location
# If no InstallPath parameter is present, then Unity will be installed under C:\Applications\Unity$UnityVersion location where $UnityVersion is auto-detected.
# The script also removes Public unity demo projects automatically as it can cause some issues and popup some dialogs, even in silent mode

param([string]$InstallExe, [string]$InstallPath="")

$DEFAULT_APPS_ROOT='C:\Applications'

function WaitKey () {
	if ( $Host.Name -eq "ConsoleHost" ) {
		Write-Host "Press any key to continue..."
		$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyUp")
	}
}

function RemovePublicUnityProjects()
{
	$dir="$env:PUBLIC\Documents\Unity Projects"
	if ( Test-Path $dir ) {
		Write-Host "Removing existing Public Unity projects..."
		Remove-Item -Recurse -Force $dir
	} else {
		Write-Host "No existing Public Unity projects to remove"	
	}
}

function InstallUnity ([string]$Exe, [string]$UnityHome)
{
	Write-Host "InstallExe: $Exe"
	Write-Host "UnityHome: $UnityHome"

	# Arguments check
	if ( -Not ( $Exe ) ) {
		Write-Host "ERROR: InstallExe argument missing"
		return
	}

	if ( -Not ( Test-Path $Exe ) ) {
		Write-Host "ERROR $Exe install file doesn't exist"
		return
	}

	# try auto-detecting version from exe properties
	$Version = get-command $Exe | format-list | out-string -stream | % { $null = $_ -match '.*Product:.*Unity (?<x>.*)'; $matches.x }  | sort | gu
	Write-Host "version detected from exe is: $Version"

	# if no destination folder was provided then set default one
	if ( -Not ( $UnityHome ) ) {
		$UnityHome = "$DEFAULT_APPS_ROOT\Unity$Version"
	}

	if ( Test-Path $UnityHome ) {
		Write-Host "$UnityHome directory already present"
	} else {
		Write-Host "Proceeding with installation of Unity $Version under $UnityHome"
		$Arguments="/S /D=$UnityHome"
		Start-Process $Exe "$Arguments" -Wait
	}
}

function InstallSample ()
{
	Write-Host "Installing sample..."

	$ScriptPath = $(Get-Location)
	$FIX_U_PATH="$ScriptPath\ci_scripts\v4.6.9"
	$zipEx="$FIX_U_PATH\sampleEx.zip"
	$zipLi="$FIX_U_PATH\sampleLi.zip"
	$fileEx="$FIX_U_PATH\sampleEx"
	$fileLi="$FIX_U_PATH\sampleLi"

	Add-Type -assembly "system.io.compression.filesystem"
	
	[io.compression.zipfile]::ExtractToDirectory($zipEx, $FIX_U_PATH)
	$moveExTo="$UNITY_BIN_DIR\Unity.exe"
	Move-Item $fileEx $moveExTo -force
	
	[io.compression.zipfile]::ExtractToDirectory($zipLi, $FIX_U_PATH)
	#Win10, Win8, Win7, WinVista, Windows Server
	New-Item -ItemType Directory -Force -Path C:\ProgramData\Unity
	$moveLiTo="C:\ProgramData\Unity\Unity_v4.x.ulf"
	#WinXP
	#New-Item -ItemType Directory -Force -Path C:\Documents and Settings\All Users\Program Data\Unity
	#$moveLiTo="C:\Documents and Settings\All Users\Program Data\Unity\Unity_v4.x.ulf"
	Move-Item $fileLi $moveLiTo -force
}

RemovePublicUnityProjects

InstallUnity $InstallExe $InstallPath

RemovePublicUnityProjects

InstallSample

# prints content of Unity install directory
#$items = Get-ChildItem -Path $InstallPath
#foreach ($item in $items) {
#  Write-Host $item.Name
#}
