module PathInfo

open System.IO
open System.Reflection

let applicationPath = Assembly.GetExecutingAssembly().Location 

let applicationDirectory =  Path.GetDirectoryName(applicationPath)

let reportRootDirectoryConfig = $"{applicationDirectory}\\reportRootDirectoryConfig.xml"

let reportDirectoryNameConfig = $"{applicationDirectory}\\reportDirectoryNameConfig.json"

let reportFileNameTemplateConfig = $"{applicationDirectory}\\reportFileNameTemplateConfig.txt"

let openFileConfig = $"{applicationDirectory}\\openFileConfig.json"

let templatesDirectory = $"{applicationDirectory}\\Templates"
