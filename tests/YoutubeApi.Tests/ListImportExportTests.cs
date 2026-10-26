using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using OfficeOpenXml;
using YoutubeApi.Domain.Entities.ExportData;
using YoutubeApi.Infrastructure.IO;

namespace YoutubeApi.Tests;

// The export helpers write to wwwroot/files relative to the working directory, so these tests run sequentially.
[Collection(FileSystemCollection.Name)]
public class ListImportExportTests : IDisposable
{
    private readonly string _tempDir = Directory.CreateTempSubdirectory("youtubeapi-tests").FullName;

    static ListImportExportTests()
    {
        ExcelPackage.License.SetNonCommercialPersonal("YoutubeApi.Tests");
    }

    public ListImportExportTests()
    {
        Directory.CreateDirectory(Path.Combine("wwwroot", "files"));
    }

    public void Dispose() => Directory.Delete(_tempDir, recursive: true);

    private record ExportRow(string Name, int Count, string? Optional);

    [Fact]
    public void GetValuesFromCsvFile_SplitsByComma()
    {
        var path = Path.Combine(_tempDir, "ids.csv");
        File.WriteAllText(path, "id1,id2,id3");

        var values = ListImportHelper.GetValuesFromCsvFile(path);

        Assert.Equal(new[] { "id1", "id2", "id3" }, values);
    }

    [Fact]
    public void GetValuesFromCsvFile_MissingFile_Throws()
    {
        Assert.Throws<FileNotFoundException>(() => ListImportHelper.GetValuesFromCsvFile(Path.Combine(_tempDir, "missing.csv")));
    }

    [Fact]
    public void GetColumnValuesFromExcelFile_SkipsHeaderAndReadsColumn()
    {
        var path = Path.Combine(_tempDir, "ids.xlsx");
        using (var package = new ExcelPackage())
        {
            var ws = package.Workbook.Worksheets.Add("Sheet1");
            ws.Cells["A1"].Value = "Header";
            ws.Cells["B1"].Value = "VideoId";
            ws.Cells["B2"].Value = "vid1";
            ws.Cells["B3"].Value = "vid2";
            ws.Cells["A3"].Value = "x";
            package.SaveAs(new FileInfo(path));
        }

        var values = ListImportHelper.GetColumnValuesFromExcelFileIgnoreFormularErrors(path, "B");

        Assert.Equal(new[] { "vid1", "vid2" }, values);
    }

    [Fact]
    public async Task ExportToExcel_WritesHeaderAndRows()
    {
        var rows = new List<ExportRow> { new("a", 1, null), new("b", 2, "opt") };

        var filePath = await ListExportHelper.ExportToExcel(rows, Path.Combine(_tempDir, "export"));

        using var doc = SpreadsheetDocument.Open(filePath, isEditable: false);
        var sheetRows = doc.WorkbookPart!.WorksheetParts.First().Worksheet.GetFirstChild<SheetData>()!.Elements<Row>()
            .Select(r => r.Elements<Cell>().Select(c => c.CellValue!.Text).ToArray())
            .ToList();
        Assert.Equal(3, sheetRows.Count);
        Assert.Equal(new[] { "Name", "Count", "Optional" }, sheetRows[0]);
        Assert.Equal(new[] { "a", "1", "" }, sheetRows[1]);
        Assert.Equal(new[] { "b", "2", "opt" }, sheetRows[2]);
    }

    [Fact]
    public async Task ExportToCsv_WritesHeaderAndRows()
    {
        var rows = new List<ExportRow> { new("a", 1, null), new("b", 2, "opt") };
        var name = $"csv-{Guid.NewGuid()}";

        var filePath = await ListExportHelper.ExportToCsv(rows, name);

        try
        {
            var lines = File.ReadAllLines(filePath);
            Assert.Equal(new[] { "Name,Count,Optional", "a,1,", "b,2,opt" }, lines);
        }
        finally
        {
            File.Delete(filePath);
        }
    }

    [Fact]
    public void ExportToSketchXml_WritesOneDocElementPerNode()
    {
        var nodes = new List<DocNode>
        {
            new() { Attributes = new YoutubeCommentVideoDetails { VideoId = "v1", CommentId = "c1" }, Value = "first <comment>" },
            new() { Attributes = new YoutubeCommentVideoDetails { VideoId = "v1", CommentId = "c2" }, Value = "second" },
        };
        var name = $"sketch-{Guid.NewGuid()}";

        var filePath = ListExportHelper.ExportToSketchXml(nodes, name);

        try
        {
            var lines = File.ReadAllLines(filePath).Where(l => l.Length > 0).ToArray();
            Assert.Equal(2, lines.Length);
            Assert.StartsWith("<doc ", lines[0]);
            Assert.Contains("VideoId=\"v1\"", lines[0]);
            Assert.Contains("CommentId=\"c1\"", lines[0]);
            Assert.EndsWith(">first &lt;comment&gt;</doc>", lines[0]);
            Assert.Contains("CommentId=\"c2\"", lines[1]);
        }
        finally
        {
            File.Delete(filePath);
        }
    }
}
