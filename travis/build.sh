#! /bin/sh

project="$APP_NAME"
unity=/Applications/Unity/Unity.app/Contents/MacOS/Unity

echo "Attempting to build $project for Windows"
$unity \
  -batchmode \
  -nographics \
  -silent-crashes \
  -logFile $(pwd)/unity.log \
  -projectPath $(pwd) \
  -buildWindowsPlayer "$(pwd)/build/windows/$project.exe" \
  -quit

echo "Attempting to build $project for OS X"
$unity \
  -batchmode \
  -nographics \
  -silent-crashes \
  -logFile $(pwd)/unity.log \
  -projectPath $(pwd) \
  -buildOSXPlayer "$(pwd)/build/osx/$project.app" \
  -quit

echo "Attempting to build $project for Linux"
$unity \
  -batchmode \
  -nographics \
  -silent-crashes \
  -logFile $(pwd)/unity.log \
  -projectPath $(pwd) \
  -buildLinux32Player "$(pwd)/build/linux/$project.exe" \
  -quit

echo 'Logs from build'
cat $(pwd)/unity.log