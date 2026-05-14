// For more information see https://aka.ms/fsharp-console-apps

open System
open System.IO

let printVersionInfo () =
    printfn ""
    printfn "%s Version.%s" VersionInfo.appName VersionInfo.appVersion
    printfn "%s" VersionInfo.copyright
    printfn "System: %s" VersionInfo.systemVersion
    printfn "Runtime: %s\n" VersionInfo.runtimeVersion

let isValidObjectName (objectName: string) =
    let invalidLetters = [ '/'; '?'; '<'; '>'; '\\'; ':'; '*'; '|'; '"' ]
    let result = String.exists (fun c -> List.contains c invalidLetters) objectName
    if result then false else true

let createReportDirectory (reportRootDir: string) (subjectDirName: string) =
    if Directory.Exists($"{reportRootDir}\\{subjectDirName}") then 
        Directory.CreateDirectory($"{reportRootDir}\\{subjectDirName}") |> ignore
    printfn ""
    printfn "Select report type"
    printfn "[0] Numbering Report (One of several reports imposed in the semester)"
    printfn "[1] Midterm Report (A report assigned only once in the middle of the semester.)"
    printfn "[2] Final Report (A report assigned only once at the end of the semester.)"
    printfn "[3] Other"
    let operationInput = Console.ReadLine()
    if operationInput = "0" then
        ReportFileHandler.makeNumberingReportDir reportRootDir subjectDirName
    elif operationInput = "1" then
        ReportFileHandler.makeMidtermReportDir reportRootDir subjectDirName
    elif operationInput = "2" then
        ReportFileHandler.makeFinalReportDir reportRootDir subjectDirName
    else
        printfn "Enter report directory name:"
        let mutable dirNameInput = Console.ReadLine()
        while isValidObjectName dirNameInput = false do
            printfn "'%s' is invalid directory name." dirNameInput
            printfn "Cannot use : /?<>\\:*|\""
            printfn "Enter report directory name again:"
            dirNameInput <- Console.ReadLine()
        ReportFileHandler.makeReportDir reportRootDir subjectDirName dirNameInput

let rec makeReportFile (reportRootDir: string) (reportDir) =
    let templateFiles = TemplateFileHandler.getTemplatesFiles()
    printfn ""
    printfn "Choose template:"
    let mutable i = 0
    while i < templateFiles.Length do
        let templateFileName = Path.GetFileName(templateFiles[i])
        printfn "[%d] %s" i templateFileName
        i <- i + 1
    let operationInput = Console.ReadLine()
    let parseResult, index = Int32.TryParse(operationInput)
    if parseResult && index < templateFiles.Length then
        let selectedTemplateFile = templateFiles[index]
        let templateFileName = Path.GetFileName(selectedTemplateFile)
        printfn "Enter report file name (Without Extension):"
        let reportFileName = Console.ReadLine()
        ReportFileHandler.makeReportFileFromTemplate reportDir templateFileName reportFileName
    else
        printfn "Invalid selection."
        printfn "Please try again.\n"
        makeReportFile reportRootDir reportDir

let openReportFile (reportFilePath: string) =
    printfn ""
    printfn "%s" reportFilePath

let rec startNewReport (reportRootDir: string) =
    let subjectDirNameDictionary = SettingsConfigurator.getReportDirNameDictionary()
    printfn ""
    let mutable i = 0
    let subjects = new ResizeArray<string>()
    for item in subjectDirNameDictionary do
        printfn "[%d] %s" i item.Key
        subjects.Add item.Key
        i <- i + 1
    printfn "[n] New Subject"
    printfn "[x] Cancel"
    printfn "Choose a subject or an operation:"
    let operationInput = Console.ReadLine()
    let parseResult, operationIndex = Int32.TryParse(operationInput)
    if parseResult then
        let subject = subjects[operationIndex]
        let subjectDirName = subjectDirNameDictionary[subject]
        createReportDirectory reportRootDir subjectDirName |> makeReportFile reportRootDir |> openReportFile
    elif operationInput = "n" then
        printfn "Enter new subject name:"
        let newSubjectName = Console.ReadLine()
        printfn "Enter new subject directory name:"
        let newSubDirName = Console.ReadLine()
        let result = subjectDirNameDictionary.TryAdd(newSubjectName, newSubDirName)
        if result then
            SettingsConfigurator.saveReportDirNameConfig subjectDirNameDictionary
            createReportDirectory reportRootDir newSubDirName |> makeReportFile reportRootDir |> openReportFile
        else
            printfn "Cannot add subject '%s'" newSubjectName
            printfn "Please try again."
            printfn "Press any key to continue...\n"
            Console.ReadKey() |> ignore
            startNewReport reportRootDir
    elif operationInput = "x" then
        printfn "Operation canceled.\n"
    else
        printfn "Invalid selection.\n"
        printfn "Press any key to continue...\n"
        Console.ReadKey() |> ignore
        startNewReport reportRootDir


let addTemplateFile (autoOverwrite: bool) =
    let mutable answerInput = "y"
    while answerInput.ToLower() = "y" do
        printfn "Enter file path to copy into templates folder:"
        let sourceFile = Console.ReadLine()
        if String.IsNullOrEmpty(sourceFile) = false && File.Exists(sourceFile) then
            if autoOverwrite = false && TemplateFileHandler.templateFileExists sourceFile then
                printfn "'%s' already exists. Overwrite this file ? [y/N]" sourceFile
                let input = Console.ReadLine()
                if input.ToLower() = "y" then 
                    TemplateFileHandler.copyFileIntoTemplates(sourceFile) |> printfn "Copied!: %s\n"
                else
                    printfn "Canceled.\n"
            else
                TemplateFileHandler.copyFileIntoTemplates(sourceFile) |> printfn "Copied!: %s\n"
            printfn "Copy another file into templates folder ? [y/N]:"
            answerInput <- Console.ReadLine()
        else
            printfn "The input is empty or such file does not exist."


// Entry Point as follow: 

printfn "%s ver.%s\n" VersionInfo.appName VersionInfo.appVersion
printfn "App location: %s\n" PathInfo.applicationDirectory

if Directory.Exists(PathInfo.templatesDirectory) = false then Directory.CreateDirectory(PathInfo.templatesDirectory) |> ignore

let reportRootDirectory = 
    if File.Exists(PathInfo.reportRootDirectoryConfig) = false then
        printfn "Enter report root directory path:"
        let mutable reportRootDirInput = Console.ReadLine()

        while String.IsNullOrEmpty(reportRootDirInput) = true || Directory.Exists(reportRootDirInput) = false do
            printfn ""
            printfn "The input is empty or such directory does not exist."
            printfn "Enter report root directory path again:"
            reportRootDirInput <- Console.ReadLine()
    
        SettingsConfigurator.saveReportRootDirConf reportRootDirInput
        reportRootDirInput
    else
        SettingsConfigurator.gerReportRootDir()

printfn "Report root directory: %s\n" reportRootDirectory

let mutable templatesFiles = TemplateFileHandler.getTemplatesFiles()
while templatesFiles.Length = 0 do
    printfn "Templates folder is empty."
    printfn "Put some files into templates folder.\n"

    addTemplateFile true

    templatesFiles <- TemplateFileHandler.getTemplatesFiles()

while true do
    printfn "Select operation:"
    printfn "[0] New Report (Start writing a report)"
    printfn "[3] Open File Configuration (Set the application to open files)"
    printfn "[4] Add Template File"
    printfn "[5] Open Templates Folder in Explorer"
    printfn "[v] Version Information"
    printfn "[x] Exit Application"

    let operationInput = Console.ReadLine().ToLower()
    if operationInput = "x" then
        exit 0
    elif operationInput = "v" then
        printVersionInfo()
    elif operationInput = "0" then
        startNewReport reportRootDirectory
    elif operationInput = "3" then
        printfn "Open File Config."
    elif operationInput = "4" then
        addTemplateFile false
    elif operationInput = "5" then
        TemplateFileHandler.openTemplatesFolder() |> ignore
    else
        printfn "Invalid selection."
