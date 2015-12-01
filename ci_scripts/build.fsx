#r @"../packages/FAKE/tools/FakeLib.dll"
#load "build-helpers.fsx"
open Fake
open System
open BuildHelpers

//Environment variables
let version = "1.0.0"
let build = environVarOrDefault "BUILD_NUMBER" "1"
let versionNumber = (version + "." + build)
setEnvironVar "VERSION_NUMBER" versionNumber

// iOS stuff
//let provisioningId = "eceff215-f35c-45a6-bed8-09bb562401e9"
let provisioningId = "GUID-here"
let provisioningName = "GenericInHouse"

// winRT stuff
let windowsSolution = "win/WinRT/YourGame.sln"
let parameters = [
    ("Configuration", "Release")
    ("AppxBundle", "Auto")
    ("AppxPackage", "True")
    ("AppxPackageIsForStore", "True")
    ("AppxPackageDir", "../../../../build/")
    ("AppxPackageDirWasSpecified", "True")
    ("AppVersion", versionNumber)
]

Target "clean" (fun () ->
    DeleteFile "TestResults.xml"
    CleanDir "ios"
	CleanDir "win"
    CleanDir "build"
    CleanDir "bin"
    CleanDir "obj"
)

Target "android" (fun () ->
    Unity "-executeMethod BuildScript.Android"
)

Target "ios-player" (fun () ->
    Unity "-executeMethod BuildScript.iOS"
)

Target "ios" (fun () ->
    // Xcode archive
    DeleteDir "ios/Xcode/UnityBuild.xarchive/"
    // Update Info.plist
    UpdatePlist version versionNumber "ios/Xcode"
    Xcode ("-project ios/Xcode/Unity-iPhone.xcodeproj -scheme Unity-iPhone archive -archivePath ios/Xcode/UnityBuild PROVISIONING_PROFILE=" + provisioningId)
    // Export the archive to an ipa file
    DeleteFile "build/UnityBuild.ipa"
    CreateDir "build"
    Xcode ("-exportArchive -archivePath ios/Xcode/UnityBuild.xcarchive -exportPath build/UnityBuild.ipa -exportProvisioningProfile " + provisioningName)
)

Target "ios64-player" (fun () ->
    Unity "-executeMethod BuildScript.iOS64"
)

Target "ios64" (fun () ->
    DeleteFile "build/UnityBuild-64.ipa"
    CreateDir "build"
    Xcode ("-exportArchive -archivePath ios/Xcode/UnityBuild.xcarchive -exportPath build/UnityBuild-64.ipa -exportProvisioningProfile " + provisioningName)
)

Target "winrt-player" (fun () ->
    Unity "-executeMethod BuildScript.WinRT"
)

Target "winrt" (fun () ->
    RestorePackages windowsSolution
    MSBuild "" "Build" (("Platform", "x86") :: parameters) [ windowsSolution ] |> ignore
    MSBuild "" "Build" (("Platform", "ARM") :: parameters) [ windowsSolution ] |> ignore
)

RunTarget()