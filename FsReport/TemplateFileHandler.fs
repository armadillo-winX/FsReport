module TemplateFileHandler

open System.Diagnostics
open System.IO

let getTemplatesFiles () =
    Directory.GetFiles(PathInfo.templatesDirectory, "*", SearchOption.TopDirectoryOnly)

let copyFileIntoTemplates (sourceFile: string) =
    let fileName = Path.GetFileName(sourceFile)
    File.Copy(sourceFile, $"{PathInfo.templatesDirectory}\\{fileName}")
    $"{PathInfo.templatesDirectory}\\{fileName}"

let templateFileExists (fileName: string) =
    File.Exists($"{PathInfo.templatesDirectory}\\{fileName}")

let openTemplatesFolder () = 
    let psi = new ProcessStartInfo()
    psi.FileName <- "explorer.exe"
    psi.Arguments <- PathInfo.templatesDirectory
    Process.Start(psi)