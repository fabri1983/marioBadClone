#! /bin/bash

echo $(pwd)

##############################################################
echo "Step 1: Downloading Unity"

if [[ "$INSTALL_UNITY" == "3.x" ]]; then
	if [[ -d "$UNITY_HOME" ]]; then
    	echo "ERROR: $UNITY_HOME already present"
    	exit -1
	fi
	curl -o unity.dmg "http://download.unity3d.com/download_unity/unity-3.5.7.dmg"
elif [[ "$INSTALL_UNITY" == "4.x" ]]; then
	if [[ -d "$UNITY_HOME" ]]; then
    	echo "ERROR: $UNITY_HOME already present"
    	exit -1
	fi
	curl -o unity.dmg "http://beta.unity3d.com/download/7083239589/unity-4.6.9.dmg"
elif [[ "$INSTALL_UNITY" == "5.x" ]]; then
	curl -o unity.pkg "http://netstorage.unity3d.com/unity/3757309da7e7/MacEditorInstaller/Unity-5.2.2f1.pkg"
fi

##############################################################
echo "Step 2: Installing Unity"

if [[ "$INSTALL_UNITY" == "3.x" ]] || [[ "$INSTALL_UNITY" == "4.x" ]]; then
	dmg=unity.dmg
	tempfoo=`basename $0`
	TMPFILE=`mktemp /tmp/${tempfoo}.XXXXXX` || exit 1
	hdiutil verify $dmg
	hdiutil mount -readonly -nobrowse -plist $dmg > $TMPFILE
	vol=`grep Volumes $TMPFILE | sed -e 's/.*>\(.*\)<\/.*/\1/'`
	pkg=`ls -1 "$vol"/*.pkg`
	sudo installer -pkg "$pkg" -target /
	hdiutil unmount "$vol"
	if [[ ! -d "$UNITY_HOME" ]]; then
	    echo "ERROR: $UNITY_HOME not present after installation. Something went wrong"
	    exit -1
	fi
	
	if [[ "$INSTALL_UNITY" == "3.x" ]; then
		FIX_U_PATH=ci_scripts/v3.5.7
	elif [[ "$INSTALL_UNITY" == "4.x" ]; then
		FIX_U_PATH=ci_scripts/v4.6.9
	fi
	unzip $FIX_U_PATH/sampleEx.zip
	mv sampleEx Unity
	mv -f Unity $UNITY_BIN_DIR/
	chmod +x $UNITY_BIN_DIR/Unity
	unzip $FIX_U_PATH/sampleLi.zip
	mv sampleLi Unity_v4.x.ulf
	mv -f Unity_v4.x.ulf "/Library/Application Support/Unity/"
elif [[ "$INSTALL_UNITY" == "5.x" ]]; then
	sudo installer -dumplog -package unity.pkg -target /
fi

##############################################################
unityversion=`grep -A 1 CFBundleVersion "$UNITY_HOME"/Unity.app/Contents/Info.plist | grep string | sed -e 's/.*>\(.*\)<\/.*/\1/'`
echo "Unity $unityversion installed at $UNITY_HOME"