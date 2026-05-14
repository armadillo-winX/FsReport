module VersionInfo

open System
open System.Diagnostics
open System.Runtime.InteropServices

let appName = FileVersionInfo.GetVersionInfo(PathInfo.applicationPath).ProductName

let appVersion = FileVersionInfo.GetVersionInfo(PathInfo.applicationPath).ProductVersion

let developerName = FileVersionInfo.GetVersionInfo(PathInfo.applicationPath).CompanyName

let copyright = FileVersionInfo.GetVersionInfo(PathInfo.applicationPath).LegalCopyright

let systemVersion = Environment.OSVersion.ToString()

let runtimeVersion = RuntimeInformation.FrameworkDescription
