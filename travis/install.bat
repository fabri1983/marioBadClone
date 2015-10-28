@echo OFF

:: Example usage:
:: - install in standard directory
::     install.bat UnitySetup-3.5.7.exe
:: - install in custom directory
::     install.bat UnitySetup-3.5.7.exe C:\Apps\Unity3.5.7

set installfile=%~f1
set ipath=%2

PowerShell -NoProfile -ExecutionPolicy Bypass -Command "&'%~dpn0.ps1' -InstallExe '%installfile%' -InstallPath '%ipath%'"
:: Note: when running in admin mode, empty arguments + 4 double quotes cause " to be passed instead of empty string ?!
::PowerShell -NoProfile -ExecutionPolicy Bypass -Command "& {Start-Process PowerShell -ArgumentList '-NoProfile -ExecutionPolicy Bypass -File """"%~dpn0.ps1"""" -InstallExe """"%installfile%"""" -InstallPath """"%ipath%"""" ' -Verb RunAs}"