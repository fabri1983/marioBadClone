#if (UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_5 || UNITY_4_6)
#define IS_UNITY_4
#endif

using UnityEditor;
using System.Collections.Generic;
using System;
using System.IO;

/// <summary>
/// Helper for building players using Unity command line interface.
/// </summary>
public class BuildScript {

	private static readonly string _versionNumber;
	private static readonly string _buildNumber;

	static BuildScript()
	{
		_versionNumber = Environment.GetEnvironmentVariable("VERSION_NUMBER");
		if (string.IsNullOrEmpty(_versionNumber))
			_versionNumber = "1.0.0"; // default version
		
		_buildNumber = Environment.GetEnvironmentVariable("BUILD_NUMBER");
		if (string.IsNullOrEmpty(_buildNumber))
			_buildNumber = "1"; // default build number
		
		PlayerSettings.productName = "MarioBadClone";
		PlayerSettings.bundleVersion = _versionNumber;
		#if IS_UNITY_4
		PlayerSettings.shortBundleVersion = _versionNumber;
		#endif
	}

	static void Android()
	{
		int versionCode;
		int.TryParse(_buildNumber, out versionCode);
		PlayerSettings.Android.bundleVersionCode = versionCode;

		PlayerSettings.Android.keyaliasName = "YourAlias";
		PlayerSettings.Android.keyaliasPass = "YourPassword";
		PlayerSettings.Android.keystorePass = "YourPassword";
		PlayerSettings.Android.keystoreName = Path.GetFullPath(@"path\to\your.keystore").Replace('\\', '/');

		string packageName = "marioBadClone_" + _versionNumber + "_" + _buildNumber + ".apk";
		BuildPipeline.BuildPlayer(GetScenes(), "build/" + packageName, BuildTarget.Android, BuildOptions.None);
	}

	static void iOS()
	{
		CreateDir("ios/Xcode");
		#if IS_UNITY_4
		PlayerSettings.SetPropertyInt("ScriptingBackend", (int)ScriptingImplementation.Mono2x, BuildTargetGroup.iPhone);
		PlayerSettings.SetPropertyInt("Architecture", (int)iPhoneArchitecture.ARMv7, BuildTargetGroup.iPhone);
		#endif
		BuildPipeline.BuildPlayer(GetScenes(), "ios/Xcode", BuildTarget.iPhone, BuildOptions.None);
	}

	static void iOS64()
	{
		CreateDir("ios/Xcode");
		#if IS_UNITY_4
		PlayerSettings.SetPropertyInt("ScriptingBackend", (int)ScriptingImplementation.IL2CPP, BuildTargetGroup.iPhone);
		PlayerSettings.SetPropertyInt("Architecture", (int)iPhoneArchitecture.Universal, BuildTargetGroup.iPhone);
		#endif
		BuildPipeline.BuildPlayer(GetScenes(), "ios/Xcode", BuildTarget.iPhone, BuildOptions.None);
	}

	static void WinRT()
	{
		#if IS_UNITY_4
		CreateDir(@"win\WinRT");
		PlayerSettings.Metro.packageVersion = new Version(_versionNumber);
		EditorUserBuildSettings.metroBuildType = MetroBuildType.VisualStudioCSharp;
		EditorUserBuildSettings.metroSDK = MetroSDK.UniversalSDK81;
		BuildPipeline.BuildPlayer(GetScenes(), "win/WinRT", BuildTarget.MetroPlayer, BuildOptions.None);
		#endif
	}

	static string[] GetScenes()
	{
		EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
		List<string> enabledScenes = new List<string>(scenes.Length);

		foreach (EditorBuildSettingsScene scene in scenes) {
			if (scene.enabled)
				enabledScenes.Add(scene.path);
		}

		return enabledScenes.ToArray();
	}

	static void CreateDir(string dir)
	{
		if (!Directory.Exists(dir))
			Directory.CreateDirectory(dir);
	}

}
