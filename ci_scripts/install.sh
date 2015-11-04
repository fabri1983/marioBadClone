#! /bin/sh

echo $(pwd)

echo "Step 1: Downloading from ${UNITY_PKG_URL}"
unityhome=/Applications/Unity
# Unity 3.x or 4.x only:
if [[ -d "$unityhome" ]]; then
    echo "ERROR: $unityhome already present"
    exit -1
fi
curl -o unity.dmg $UNITY_PKG_URL
# Unity 5.x only:
#curl -o unity.pkg $UNITY_PKG_URL

echo "Step 2: Installing Unity"
# For Unity 3.x or 4.x use next:
# install Unity3d given a dmg file. Resulting file is stored under /Applications/Unity
dmg=unity.dmg
tempfoo=`basename $0`
TMPFILE=`mktemp /tmp/${tempfoo}.XXXXXX` || exit 1
hdiutil verify $dmg
hdiutil mount -readonly -nobrowse -plist $dmg > $TMPFILE
vol=`grep Volumes $TMPFILE | sed -e 's/.*>\(.*\)<\/.*/\1/'`
pkg=`ls -1 "$vol"/*.pkg`
sudo installer -pkg "$pkg" -target /
hdiutil unmount "$vol"
if [[ ! -d "$unityhome" ]]; then
    echo "ERROR: $unityhome not present after installation. Something went wrong"
    exit -1
fi
unzip ci_scripts/v4.6.3/sampleEx.zip
mv sampleEx Unity
mv -f Unity "$unityhome/Unity.app/Contents/MacOS/"
chmod +x "$unityhome/Unity.app/Contents/MacOS/Unity"
ls "$unityhome/Unity.app/Contents/MacOS/"
unzip ci_scripts/v4.6.3/sampleLi.zip
mv sampleLi Unity_v4.x.ulf
mv -f Unity_v4.x.ulf "/Library/Application Support/Unity/"
# For Unity 5.x use only next:
#sudo installer -dumplog -package unity.pkg -target /

unityversion=`grep -A 1 CFBundleVersion "$unityhome"/Unity.app/Contents/Info.plist | grep string | sed -e 's/.*>\(.*\)<\/.*/\1/'`
echo "Unity $unityversion installed at $unityhome"