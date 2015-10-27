#! /bin/sh

echo 'Downloading from $UNITY_PKG_URL'
# Unity 3.x only:
unityhome=/Applications/Unity
if [[ -d "$unityhome" ]]; then
    echo "ERROR: $unityhome already present"
    exit -1
fi
curl -o unity.dmg $UNITY_PKG_URL
# Unity 5.x only:
#curl -o unity.pkg $UNITY_PKG_URL


echo 'Installing Unity.pkg'
# For Unity 3.x use next:
# install Unity3d automatically from the command line given a dmg file. Resulting file is stored under /Applications/Unity
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
unityversion=`grep -A 1 CFBundleVersion "$unityhome"/Unity.app/Contents/Info.plist | grep string | sed -e 's/.*>\(.*\)<\/.*/\1/'`
sudo mv "$unityhome" "$unityhome$unityversion"
echo "Unity $unityversion installed at $unityhome"
# For Unity 5.x use only next:
#sudo installer -dumplog -package unity.pkg -target /