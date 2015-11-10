# A PowerShell script to automate the installation of the specified Unity in a standard location
# If no parameter is present, then Unity will be installed under C:\Applications\Unity$UnityVersion location and $UnityVersion is auto-detected.
# The script also removes Public unity demo projects automatically as it can cause some issues and popup some dialogs, even in silent mode
# The PowerShell script is wrapped by a BAT file for easy execution

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

	# try auto-detecting version from exe properties, if no destination folder was provided
	if ( -Not ( $UnityHome ) ) {
		Write-Host "Auto-detecting version..."
		$Version = get-command $Exe | format-list | out-string -stream | % { $null = $_ -match '.*Product:.*Unity (?<x>.*)'; $matches.x }  | sort | gu
		$UnityHome = "$DEFAULT_APPS_ROOT\Unity$Version"
	}

	if ( Test-Path $UnityHome ) {
		Write-Host "$UnityHome directory already present"
		return
	} else {
		Write-Host "Proceeding with installation of Unity $Version under $UnityHome"
	}

	$Arguments="/S /D=$UnityHome"
	Start-Process $Exe "$Arguments" -Wait
}

function InstallSample ([string]$SampleLi, [string]$SampleLi)
{
	Write-Host "Installing sample... $SampleEx  $SampleLi"
	return
}

RemovePublicUnityProjects

InstallUnity $InstallExe $InstallPath

RemovePublicUnityProjects

InstallSample "sampleEx" "sampleLi"

# prints content of directory
$items = Get-ChildItem -Path $InstallPath
foreach ($item in $items) {
  Write-Host $item.Name
}
