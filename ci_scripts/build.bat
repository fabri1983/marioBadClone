@echo off

:: USERPROFILE is user's hOME environment variable
echo %USERPROFILE%

set PROJECT_PATH="%USERPROFILE%\Documents\Unity Projects\%APP_NAME%\build"
set WIN32_PATH="%PROJECT_PATH%\win32\%APP_NAME%.exe"
set OSX32_PATH="%PROJECT_PATH%\osx\%APP_NAME%.app"
set LINUX32_PATH="%PROJECT_PATH%\linux\%APP_NAME%.app"

:: Common options
set PROJECT=-projectPath 
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
