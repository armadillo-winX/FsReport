module ReportFileHandler

open System.Diagnostics
open System.IO

let makeReportDir (reportRootDir: string) (subjectDirName: string) (directoryName) =
    let reportDir = $"{reportRootDir}\\{subjectDirName}\\{directoryName}"
    Directory.CreateDirectory(reportDir) |> ignore
    reportDir

let makeNumberingReportDir (reportRootDir: string) (subjectDirName: string) =
    let subjectDir = $"{reportRootDir}\\{subjectDirName}"
    let mutable i = 1
    let mutable numbering = i.ToString("D2")
    while Directory.Exists($"{subjectDir}\\{numbering}") do
        i <- i + 1
        numbering <- i.ToString("D2")
    makeReportDir reportRootDir subjectDirName numbering

let makeMidtermReportDir (reportRootDir: string) (subjectDirName: string) =
    makeReportDir reportRootDir subjectDirName "Midterm"

let makeFinalReportDir (reportRootDir: string) (subjectDirName: string) =
    makeReportDir reportRootDir subjectDirName "Final"

let makeReportFileFromTemplate (reportDir: string) (templateFileName: string) (reportFileNameWithoutExtension :string) =
    let templateFilePath = $"{PathInfo.templatesDirectory}\\{templateFileName}"
    let extension = Path.GetExtension(templateFilePath)
    let reportFilePath = $"{reportDir}\\{reportFileNameWithoutExtension}{extension}"
    File.Copy(templateFilePath, reportFilePath)
    reportFilePath

let openReportFile (reportFilePath: string) =
    let fileAssociationDictionary = SettingsConfigurator.getFileAssociationDictionary()
    let reportFileExtension = Path.GetExtension(reportFilePath)
    let result, associatedApplicationPath = fileAssociationDictionary.TryGetValue(reportFileExtension)
    if result = true then
        let psi = new ProcessStartInfo()
        psi.FileName <-  associatedApplicationPath
        psi.Arguments <- $"\"{reportFilePath}\""
        Process.Start(psi) |> ignore
        true
    else
        false
