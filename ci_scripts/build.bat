@echo off

:: USERPROFILE is user's hOME environment variable
echo %USERPROFILE%

set PROJECT_PATH=C:\projects\mariobadclone
set WIN32_PATH=%PROJECT_PATH%\build\win32\%APP_NAME%.exe
set OSX32_PATH=%PROJECT_PATH%\build\osx\%APP_NAME%.app
set LINUX32_PATH=%PROJECT_PATH%\build\linux\%APP_NAME%.app

:: Common options
set PROJECT=-projectPath 
set BUILD_PARAMS=-batchmode -quit -nographics -silent-crashes -logFile %PROJECT_PATH%\unity.log

:: Builds:
set WIN32=-buildWindowsPlayer %WIN32_PATH%
set OSX32=-buildOSXPlayer %OSX32_PATH%
set LINUX32=-buildLinux32Player %LINUX32_PATH%

:: Win32 build
echo Running Win Build
echo %UNITY_BIN_DIR%\Unity.exe %BUILD_PARAMS% %PROJECT% %PROJECT_PATH% %WIN32%
%UNITY_BIN_DIR%\Unity.exe %BUILD_PARAMS% %PROJECT% %PROJECT_PATH% %WIN32%
echo Logs from Win build
type %PROJECT_PATH%\unity.log
echo. > %PROJECT_PATH%\unity.log

:: OSX32 build
echo Running OSX Build
echo %UNITY_BIN_DIR%\Unity.exe %BUILD_PARAMS% %PROJECT% %PROJECT_PATH% %OSX32%
%UNITY_BIN_DIR%\Unity.exe %BUILD_PARAMS% %PROJECT% %PROJECT_PATH% %OSX32%
echo Logs from OSX build
type %PROJECT_PATH%\unity.log

:: Linux build
::echo Running Linux Build
::echo %UNITY_BIN_DIR%\Unity.exe" %BUILD_PARAMS% %PROJECT% %PROJECT_PATH% %LINUX32%
::%UNITY_BIN_DIR%\Unity.exe %BUILD_PARAMS% %PROJECT% %PROJECT_PATH% %LINUX32%
::echo Logs from Linux build
::type %PROJECT_PATH%\unity.log
::echo. > %PROJECT_PATH%\unity.log


::"packages\FAKE.4.3.7\tools\FAKE.exe" build.fsx %1%