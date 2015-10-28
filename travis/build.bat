@echo off

:: USERPROFILE is user's hOME environment variable
echo %USERPROFILE%

set PROJECT=-projectPath 
set PROJECT_PATH="%USERPROFILE%\Documents\project\Unity\UGDE"

set WIN_PATH="%USERPROFILE%\Documents\project\Unity\UGDE\build\win32\island.exe"
set OSX_PATH="%USERPROFILE%\Documents\project\Unity\UGDE\build\osx\island.app"

::With Unity 4 we now have Linux
set LINUX_PATH="%USERPROFILE%\Documents\project\Unity\UGDE\build\linux\island.app"

set LINUX64_PATH="%USERPROFILE%\Documents\project\Unity\UGDE\build\linux64\island.app"

:: Common options
set BATCH=-batchmode
set QUIT=-quit

:: Builds:
set WIN=-buildWindowsPlayer %WIN_PATH%
set OSX=-buildOSXPlayer %OSX_PATH%
set LINUX=-buildLinux32Player %LINUX_PATH%
set LINUX64=-buildLinux64Player %LINUX64_PATH%

:: Win32 build
echo "Running Win Build for: %PROJECT_PATH%"
echo "%PROGRAMFILES%\Unity\Editor\Unity.exe" %BATCH% %QUIT% %PROJECT% %PROJECT_PATH% %WIN%
"%ProgramFiles(x86)%\Unity\Editor\Unity.exe" %BATCH% %QUIT% %PROJECT% %PROJECT_PATH% %WIN%

:: OSX build
echo "Running OSX Build for: %PROJECT_PATH%"
echo "%PROGRAMFILES%\Unity\Editor\Unity.exe" %BATCH% %QUIT% %PROJECT% %PROJECT_PATH% %OSX%
"%ProgramFiles(x86)%\Unity\Editor\Unity.exe" %BATCH% %QUIT% %PROJECT% %PROJECT_PATH% %OSX%

:: Linux build
echo "Running Linux Build for: %PROJECT_PATH%"
echo "%PROGRAMFILES%\Unity\Editor\Unity.exe" %BATCH% %QUIT% %PROJECT% %PROJECT_PATH% %LINUX%
"%ProgramFiles(x86)%\Unity\Editor\Unity.exe" %BATCH% %QUIT% %PROJECT% %PROJECT_PATH% %LINUX%

:: Linux 64-bit build
echo "Running Linux Build for: %PROJECT_PATH%"
echo "%PROGRAMFILES%\Unity\Editor\Unity.exe" %BATCH% %QUIT% %PROJECT% %PROJECT_PATH% %LINUX64%
"%ProgramFiles(x86)%\Unity\Editor\Unity.exe" %BATCH% %QUIT% %PROJECT% %PROJECT_PATH% %LINUX64%
