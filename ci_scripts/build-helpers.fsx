module BuildHelpers
#r @"../packages/FAKE/tools/FakeLib.dll"

open Fake
open Fake.MSBuildHelper
open System
open System.IO
open System.Linq
open System.Xml
open System.Diagnostics

let unity_bin = Environment.GetEnvironmentVariable "UNITY_BIN"

let possibleUnityPaths = [
    unity_bin
    @unity_bin
]

let Exec command args =
    let result = Shell.Exec(command, args)
    if result <> 0 then failwithf "%s exited with error %d" command result

let RestorePackages solutionFile =
    Exec "NuGet.exe" ("restore \"" + solutionFile + "\"")

let UnityPath =
    (Seq.where(fun p -> File.Exists(p)) possibleUnityPaths).First()

let Unity args =
    let fullPath = Path.GetFullPath(".")
    let result = Shell.Exec(UnityPath, "-quit -batchmode -logFile -projectPath \"" + fullPath + "\" " + args)
    if result < 0 then failwithf "Unity exited with error %d" result

let Xcode args =
    Exec "xcodebuild" args

let UpdatePlist shortVersion version project =
    let info = Path.Combine(project, "Info.plist")
    Exec "/usr/libexec/PlistBuddy" ("-c 'Set :CFBundleShortVersionString " + shortVersion + "' " + info)
    Exec "/usr/libexec/PlistBuddy" ("-c 'Set :CFBundleVersion " + version + "' " + info)

let UpdateManifest version build path = 
    let ns = Seq.singleton(("android", "http://schemas.android.com/apk/res/android"))
    XmlPokeNS path ns "manifest/@android:versionName" (version + "." + build)
    XmlPokeNS path ns "manifest/@android:versionCode" build

let ArchiveUnityLog path =
    if (not(isUnix)) then 
        let appdata = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
        let unityLogPath = Path.Combine(appdata, "Unity", "Editor", "Editor.log")
        if File.Exists(unityLogPath) then
            CreateDir "build"
            File.Copy(unityLogPath, path, true)
            TeamCityHelper.PublishArtifact path
        else
            Console.WriteLine("Could not find Unity log: " + unityLogPath)
