using CsvHelper;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Xml;
using YoutubeApi.Domain.Entities.ExportData;

namespace YoutubeApi.Infrastructure.IO
{
    public static class ListExportHelper
    {
        public static EventHandler<ProgressChangedEventArgs> ProgressChanged { get; set; }

        // Export List to xlx sheet with DocumentFormat.OpenXml
        public static async Task<string> ExportToExcel<T>(List<T> list, string path)
        {
            string filePath = null;
            await Task.Run(() =>
            {
                filePath = string.Concat(path, ".xls");
                if (File.Exists(filePath))
                    File.Delete(filePath);
                CreateSpreadSheetDocument<T>(filePath);

                var workbook = SpreadsheetDocument.Open(filePath, isEditable: true);
                int index = 0;
                int? progress = null;
                foreach (var item in list)
                {
                    //if (index > 0 && index % 500_000 == 0)
                    //{
                    //    workbook.Save();
                    //    workbook.Dispose();
                    //    Log.Information("Disposing workbook after {count} items", index);
                    //    workbook = SpreadsheetDocument.Open(filePath, isEditable: true);
                    //}
                    AddItemToWorkbook(workbook, item);

                    index++;
                    if (progress is null || progress != index)
                    {
                        progress = (int)((double)index / list.Count * 100);
                        ProgressChanged?.Invoke(null, new ProgressChangedEventArgs(progress ?? -25, null));
                    }
                }
                workbook.Save();
                workbook.Dispose();
            });
            return filePath;
        }

        private static void AddItemToWorkbook<T>(SpreadsheetDocument workbook, T item)
        {
            var sheetPart = workbook.WorkbookPart.WorksheetParts.First();
            var sheetData = sheetPart.Worksheet.GetFirstChild<SheetData>();
            var columns = new List<string>();
            foreach (PropertyInfo property in typeof(T).GetProperties())
                columns.Add(property.Name);
            var newRow = new Row();
            foreach (string col in columns)
            {
                Cell cell = new Cell();
                cell.DataType = CellValues.String;
                var property = item.GetType().GetProperty(col);
                if (property != null && property.GetValue(item) != null)
                    cell.CellValue = new CellValue(property.GetValue(item).ToString());
                else
                    cell.CellValue = new CellValue("");
                newRow.AppendChild(cell);
            }
            sheetData.AppendChild(newRow);
        }

        private static void CreateSpreadSheetDocument<T>(string filePath)
        {
            using (var workbook = SpreadsheetDocument.Create(filePath, SpreadsheetDocumentType.Workbook))
            {
                var workbookPart = workbook.AddWorkbookPart();
                workbook.WorkbookPart.Workbook = new Workbook();
                workbook.WorkbookPart.Workbook.Sheets = new Sheets();
                var sheetPart = workbook.WorkbookPart.AddNewPart<WorksheetPart>();
                var sheetData = new SheetData();
                sheetPart.Worksheet = new Worksheet(sheetData);
                var sheets = workbook.WorkbookPart.Workbook.GetFirstChild<Sheets>();
                string relationshipId = workbook.WorkbookPart.GetIdOfPart(sheetPart);
                uint sheetId = 1;
                if (sheets.Elements<Sheet>().Count() > 0)
                    sheetId = sheets.Elements<Sheet>().Select(s => s.SheetId.Value).Max() + 1;

                var sheet = new Sheet() { Id = relationshipId, SheetId = sheetId, Name = "Sheet1" };
                sheets.Append(sheet);
                var headerRow = new Row();
                var columns = new List<string>();
                foreach (PropertyInfo property in typeof(T).GetProperties())
                {
                    columns.Add(property.Name);
                    Cell cell = new Cell();
                    cell.DataType = CellValues.String;
                    cell.CellValue = new CellValue(property.Name);
                    headerRow.AppendChild(cell);
                }
                sheetData.AppendChild(headerRow);
            }
        }

        public static async Task<string> ExportToCsv<T>(List<T> list, string path)
        {
            string filePath = null;
            await Task.Run(() =>
            {
                filePath = Path.Combine("wwwroot", "files", string.Concat(path, ".csv"));
                if (File.Exists(filePath))
                    File.Delete(filePath);
                using (var writer = new StreamWriter(filePath))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteRecords(list);
                }
            });

            return filePath;
        }

        public static string ExportToSketchXml(List<DocNode> list, string path)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.OmitXmlDeclaration = true; // Disable XML declaration
            string filePath = Path.Combine(
                    "wwwroot/files",
                    string.Concat(path, ".txt"));
            if (File.Exists(filePath))
                File.Delete(filePath);
            using (FileStream fileStream = new FileStream(
                filePath,
                FileMode.Create))
            {
                foreach (var item in list)
                {
                    var xmlDocument = new XmlDocument();
                    using (XmlWriter writer = XmlWriter.Create(fileStream, settings))
                    {
                        var rootElement = xmlDocument.CreateElement("doc");

                        var propertyObject = item.Attributes;
                        foreach (var property in propertyObject.GetType().GetProperties())
                        {
                            var propertyName = property.Name;
                            var propertyValue = property.GetValue(propertyObject)?.ToString();

                            if (!string.IsNullOrEmpty(propertyValue))
                            {
                                rootElement.SetAttribute(propertyName, propertyValue);
                            }
                        }

                        if (item.Value != null)
                        {
                            rootElement.InnerText = item.Value;
                        }

                        xmlDocument.AppendChild(rootElement);
                        xmlDocument.Save(writer);
                        writer.WriteRaw("\n"); // Force new line after each doc tag
                    }
                }
            }
            return filePath;
        }
    }
}
