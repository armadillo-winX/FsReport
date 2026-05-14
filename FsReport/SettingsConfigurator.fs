module SettingsConfigurator

open System.Collections.Generic
open System.IO
open System.Text.Encodings.Web
open System.Text.Json
open System.Text.Unicode
open System.Xml

let saveReportRootDirConf (reportRootDir: string) =
    let reportRootDirConfXml = new XmlDocument()
    reportRootDirConfXml.CreateXmlDeclaration("1.0", "UTF-8", null) |> reportRootDirConfXml.AppendChild |> ignore
    let rootNode = reportRootDirConfXml.CreateElement("ReportRootDirectoryConfig")
    reportRootDirConfXml.AppendChild(rootNode) |> ignore
    let pathNode = reportRootDirConfXml.CreateElement("Path")
    reportRootDir |> reportRootDirConfXml.CreateTextNode |> pathNode.AppendChild |> ignore
    rootNode.AppendChild(pathNode) |> ignore
    reportRootDirConfXml.Save(PathInfo.reportRootDirectoryConfig)

let gerReportRootDir () =
    let reportRootDirConfXml = new XmlDocument()
    reportRootDirConfXml.Load(PathInfo.reportRootDirectoryConfig)
    reportRootDirConfXml.SelectSingleNode("//Path").InnerText

let getReportDirNameDictionary () =
    let mutable reportDirNameDictionary = new Dictionary<string, string>()
    if File.Exists(PathInfo.reportDirectoryNameConfig) then
        let config = File.ReadAllText(PathInfo.reportDirectoryNameConfig)
        let serializeOption = new JsonSerializerOptions()
        serializeOption.Encoder <- JavaScriptEncoder.Create(UnicodeRanges.All)
        serializeOption.WriteIndented <- true
        reportDirNameDictionary <- JsonSerializer.Deserialize<Dictionary<string, string>>(config, serializeOption)
    reportDirNameDictionary

let saveReportDirNameConfig (reportDirNameDictionary: Dictionary<string, string>) =
    let reportDirNameConfig = PathInfo.reportDirectoryNameConfig
    let serializeOption = new JsonSerializerOptions()
    serializeOption.Encoder <- JavaScriptEncoder.Create(UnicodeRanges.All)
    serializeOption.WriteIndented <- true
    let jsonData = JsonSerializer.Serialize(reportDirNameDictionary, serializeOption)
    File.WriteAllText(reportDirNameConfig, jsonData)
