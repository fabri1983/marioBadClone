@echo off

:: USERPROFILE is user's hOME environment variable
echo %USERPROFILE%

set PROJECT=-projectPath 
set PROJECT_PATH="%USERPROFILE%\Documents\project\Unity\marioBadClone"

set WIN32_PATH="%USERPROFILE%\Documents\project\Unity\marioBadClone\build\win32\marioBadClone.exe"
set OSX32_PATH="%USERPROFILE%\Documents\project\Unity\marioBadClone\build\osx\marioBadClone.app"
set LINUX32_PATH="%USERPROFILE%\Documents\project\Unity\marioBadClone\build\linux\marioBadClone.app"

:: Common options
set BUILD_PARAMS="-batchmode -quit -nographics -silent-crashes"

:: Builds:
set WIN32=-buildWindowsPlayer %WIN32_PATH%
set OSX32=-buildOSXPlayer %OSX32_PATH%
set LINUX32=-buildLinux32Player %LINUX32_PATH%

:: Win32 build
echo "Running Win Build for: %PROJECT_PATH%"
echo "%PROGRAMFILES%\Unity\Editor\Unity.exe" %BUILD_PARAMS% %PROJECT% %PROJECT_PATH% %WIN32%
"%ProgramFiles(x86)%\Unity\Editor\Unity.exe" %BUILD_PARAMS% %PROJECT% %PROJECT_PATH% %WIN32%

:: OSX32 build
echo "Running OSX Build for: %PROJECT_PATH%"
echo "%PROGRAMFILES%\Unity\Editor\Unity.exe" %BUILD_PARAMS% %PROJECT% %PROJECT_PATH% %OSX32%
"%ProgramFiles(x86)%\Unity\Editor\Unity.exe" %BUILD_PARAMS% %PROJECT% %PROJECT_PATH% %OSX32%

:: Linux build
::echo "Running Linux Build for: %PROJECT_PATH%"
::echo "%PROGRAMFILES%\Unity\Editor\Unity.exe" %BUILD_PARAMS% %PROJECT% %PROJECT_PATH% %LINUX32%
::"%ProgramFiles(x86)%\Unity\Editor\Unity.exe" %BUILD_PARAMS% %PROJECT% %PROJECT_PATH% %LINUX32%
