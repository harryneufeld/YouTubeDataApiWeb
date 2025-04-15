using DocumentFormat.OpenXml.Spreadsheet;
using OfficeOpenXml;
using System.Data;

namespace YoutubeApi.Infrastructure.IO
{
    public static class ListImportHelper
    {
        public static string[] GetColumnValuesFromExcelFile(string filePath, string columnName)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("File not found", filePath);
            }
            // ExcelPackage.License.SetNonCommercialPersonal("Harry Neufeld");

            // old code
            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                var worksheet = package.Workbook.Worksheets[0]; // Assuming the first worksheet

                // Get the values of the cells in the specified column
                var columnValues = worksheet.Cells[$"{columnName}2:{columnName}{worksheet.Dimension.End.Row}"]
                    .Select(c => c.Text)
                    .ToArray();

                return columnValues;
            }
        }

        public static string[] GetColumnValuesFromExcelFileIgnoreFormularErrors(string filePath, string columnName)
        {
            string[] data;
            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                var ws = package.Workbook.Worksheets[0];
                data = new string[ws.Dimension.End.Row-1];
                var wsRow = ws.Cells[$"{columnName}2:{columnName}{ws.Dimension.End.Row}"];
                int index = 0;
                foreach (var cell in wsRow)
                {
                    if (cell.Value is ExcelErrorValue)
                        data[index] = cell.Formula;
                    else
                        data[index] = cell.Text;                    
                    index++;
                }
            }
            return data;
        }

        public static string[] GetValuesFromCsvFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("File not found", filePath);
            }

            var csvContent = File.ReadAllText(filePath);
            var values = csvContent.Split(',');

            return values;
        }
    }
}
