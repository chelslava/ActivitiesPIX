using Activities.Utilities.Properties;
using BR.Core.Attributes;
using OfficeOpenXml;

namespace Activities.Utilities;

[LocalizableScreenName(nameof(Resources.SettingFileCombined_ScreenName), typeof(Resources))]
[BR.Core.Attributes.Path("Utilities")]
public class SettingFileCombined : BR.Core.Activity
{
    // Путь к файлу Excel
    [LocalizableScreenName(nameof(Resources.Excel_FileName_ScreenName), typeof(Resources))]
    [LocalizableDescription(nameof(Resources.Excel_FileName_Description), typeof(Resources))]
    [IsFilePathChooser]
    [IsRequired]
    public string FileName { get; set; }

    // Режим выполнения алгоритма (простой или расширенный)
    [LocalizableScreenName(nameof(Resources.SettingFile_Mode_ScreenName), typeof(Resources))]
    [LocalizableDescription(nameof(Resources.SettingFile_Mode_Description), typeof(Resources))]
    [IsRequired]
    public Algorithm Mode { get; set; }

    // Диапазон для чтения данных из Excel
    [LocalizableScreenName(nameof(Resources.ExcelReadRange_Range_ScreenName), typeof(Resources))]
    [LocalizableDescription(nameof(Resources.ExcelReadRange_Range_Description), typeof(Resources))]
    public string Range { get; set; } = string.Empty;

    // Указывает, что первая строка в Excel является заголовком
    [LocalizableScreenName(nameof(Resources.ExcelReadRange_FirstRowIsHeader_ScreenName), typeof(Resources))]
    [LocalizableDescription(nameof(Resources.ExcelReadRange_FirstRowIsHeader_Description), typeof(Resources))]
    public bool WithHeaders { get; set; } = true;

    // Результат выполнения алгоритма
    [LocalizableScreenName(nameof(Resources.SettingFile_DataResult_ScreenName), typeof(Resources))]
    [LocalizableDescription(nameof(Resources.SettingFile_DataResult_Description), typeof(Resources))]
    [IsOut]
    public object Result { get; set; }

    // Метод для выполнения действий активности
    public override void Execute(int? optionID)
    {
        try
        {
            var workbook = GetWorkbook();
            var sheets = GetSheets(workbook);

            this.Result = Mode switch
            {
                Algorithm.Simple => ExecuteSimpleMode(workbook, sheets),
                Algorithm.Extended => ExecuteExtendedMode(workbook, sheets),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
        catch (Exception ex)
        {
            throw new Exception("Ошибка при выполнении действия.", ex);
        }
    }

    // Получение объекта ExcelPackage из файла
    private ExcelPackage GetWorkbook()
    {
        try
        {
            var fileInfo = new FileInfo(this.FileName);
            return new ExcelPackage(fileInfo);
        }
        catch (Exception ex)
        {
            throw new Exception("Ошибка при получении книги из файла.", ex);
        }
    }

    // Получение списка листов из книги Excel
    private IEnumerable<string> GetSheets(ExcelPackage workbook)
    {
        try
        {
            return workbook.Workbook.Worksheets.Select(worksheet => worksheet.Name);
        }
        catch (Exception ex)
        {
            throw new Exception("Ошибка при получении листов из книги.", ex);
        }
    }

    // Выполнение простого режима
    private Dictionary<string, Dictionary<string, object>> ExecuteSimpleMode(ExcelPackage workbook, IEnumerable<string> sheets)
    {
        try
        {
            return sheets.ToDictionary(sheet => sheet, sheet => ProcessSimpleSheet(workbook.Workbook.Worksheets[sheet]));
        }
        catch (Exception ex)
        {
            throw new Exception("Ошибка при выполнении простого режима.", ex);
        }
    }

    // Обработка простого листа Excel
    private Dictionary<string, object> ProcessSimpleSheet(ExcelWorksheet worksheet)
    {
        try
        {
            int startRow = WithHeaders ? worksheet.Dimension.Start.Row + 1 : worksheet.Dimension.Start.Row;

            return Enumerable.Range(startRow, worksheet.Dimension.End.Row - startRow + 1)
                .Select(row => new
                {
                    Key = worksheet.Cells[row, 1].Text,
                    Value = worksheet.Cells[row, 2].Text
                })
                .Where(x => !string.IsNullOrWhiteSpace(x.Key))
                .GroupBy(x => x.Key)
                .ToDictionary(
                    g => g.Key,
                    g => g.Count() > 1 ? (object)g.Select((x, i) => new { x.Key, x.Value, Index = i }).ToDictionary(x => $"{x.Key}_{x.Index + 1}", x => x.Value) : (object)g.First().Value
                );
        }
        catch (Exception ex)
        {
            throw new Exception("Ошибка при обработке простого листа.", ex);
        }
    }

    // Выполнение расширенного режима
    private Dictionary<string, List<Dictionary<string, object>>> ExecuteExtendedMode(ExcelPackage workbook, IEnumerable<string> sheets)
    {
        try
        {
            return sheets.ToDictionary(sheet => sheet, sheet => ProcessExtendedSheet(workbook.Workbook.Worksheets[sheet]));
        }
        catch (Exception ex)
        {
            throw new Exception("Ошибка при выполнении расширенного режима.", ex);
        }
    }

    // Обработка расширенного листа Excel
    private List<Dictionary<string, object>> ProcessExtendedSheet(ExcelWorksheet worksheet)
    {
        try
        {
            var headers = Enumerable.Range(worksheet.Dimension.Start.Column, worksheet.Dimension.End.Column - worksheet.Dimension.Start.Column + 1)
                .Select(col => worksheet.Cells[1, col].Text)
                .ToArray();

            int startRow = WithHeaders ? worksheet.Dimension.Start.Row + 1 : worksheet.Dimension.Start.Row;

            return Enumerable.Range(startRow, worksheet.Dimension.End.Row - startRow + 1)
                .Select(row => Enumerable.Range(worksheet.Dimension.Start.Column, worksheet.Dimension.End.Column - worksheet.Dimension.Start.Column + 1)
                    .Select(col => new { Header = headers[col - worksheet.Dimension.Start.Column], Value = worksheet.Cells[row, col].Text })
                    .Where(x => !string.IsNullOrWhiteSpace(x.Value))
                    .ToDictionary(x => x.Header, x => (object)x.Value))
                .Where(rowDict => rowDict.Count > 0)
                .ToList();
        }
        catch (Exception ex)
        {
            throw new Exception("Ошибка при обработке расширенного листа.", ex);
        }
    }
}