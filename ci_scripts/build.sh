#! /bin/bash

echo $(pwd)

unity=$UNITY_BIN_DIR/Unity

echo "Attempting to build $APP_NAME for Windows 32bits"
$unity \
  -batchmode \
  -nographics \
  -silent-crashes \
  -logFile $(pwd)/unity.log \
  -projectPath $(pwd) \
  -buildWindowsPlayer "$(pwd)/build/windows/$APP_NAME.exe" \
  -quit
echo "Logs from build"
cat $(pwd)/unity.log
cat /dev/null >| $(pwd)/unity.log

echo "Attempting to build $APP_NAME for OS X 32bits"
$unity \
  -batchmode \
  -nographics \
  -silent-crashes \
  -logFile $(pwd)/unity.log \
  -projectPath $(pwd) \
  -buildOSXPlayer "$(pwd)/build/osx/$APP_NAME.app" \
  -quit
echo "Logs from build"
cat $(pwd)/unity.log
cat /dev/null >| $(pwd)/unity.log

#echo "Attempting to build $APP_NAME for Linux 32bits"
#$unity \
#  -batchmode \
#  -nographics \
#  -silent-crashes \
#  -logFile $(pwd)/unity.log \
#  -projectPath $(pwd) \
#  -buildLinux32Player "$(pwd)/build/linux/$APP_NAME.app" \
#  -quit


#Exports an Xcode project to Scratch/Xcode
#mono --runtime=v4.0 packages/FAKE/tools/Fake.exe build.fsx ios-player -ev UNITY_BIN "$UNITY_BIN_DIR/Unity"
#Builds the Xcode project, and exports the ipa file to build/
#mono --runtime=v4.0 packages/FAKE/tools/Fake.exe build.fsx ios -ev UNITY_BIN "$UNITY_BIN_DIR/Unity"
#Builds an Android apk to build/
#mono --runtime=v4.0 packages/FAKE/tools/Fake.exe build.fsx android -ev UNITY_BIN "$UNITY_BIN_DIR/Unity"
