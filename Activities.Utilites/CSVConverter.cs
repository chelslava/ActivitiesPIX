using BR.Core.Attributes;
using System.Data;
using System.Text;
using System.Xml;
using Activities.Utilites.Properties;
using Newtonsoft.Json.Linq;
using System.Xml.Linq;

namespace Namespace_Utilites
{
    [LocalizableScreenName(nameof(Resources.CSVConverter_ScreenName), typeof(Resources))]
    [BR.Core.Attributes.Path("Utilites")]
    public class CSVConverter : BR.Core.Activity
    {
        [LocalizableScreenName(nameof(Resources.InputCsvFile_ScreenName), typeof(Resources))]
        [LocalizableDescription(nameof(Resources.OutputConvertedFile_ScreenName), typeof(Resources))]
        [IsRequired]
        [IsFilePathChooser]
        public BR.Core.FilePath InputCsvFile { get; set; }

        [LocalizableScreenName(nameof(Resources.OutputConvertedFile_ScreenName), typeof(Resources))]
        [LocalizableDescription(nameof(Resources.OutputConvertedFile_Description), typeof(Resources))]
        [IsRequired]
        [IsFilePathChooser]
        public BR.Core.FilePath OutputConvertedFile { get; set; }

        [LocalizableScreenName(nameof(Resources.OutputFormat_ScreenName), typeof(Resources))]
        [LocalizableDescription(nameof(Resources.OutputFormat_Description), typeof(Resources))]
        [IsRequired]
        public string OutputFormat { get; set; }

        [LocalizableScreenName(nameof(Resources.CsvDelimiter_ScreenName), typeof(Resources))]
        [LocalizableDescription(nameof(Resources.CsvDelimiter_Description), typeof(Resources))]
        [IsRequired]
        public TypeDelimiter CsvDelimiter { get; set; }

        [LocalizableScreenName(nameof(Resources.EncodingType_ScreenName), typeof(Resources))]
        [LocalizableDescription(nameof(Resources.EncodingType_Description), typeof(Resources))]
        [IsRequired]
        public TypeEncoding EncodingType { get; set; }

        [LocalizableScreenName(nameof(Resources.Success_ScreenName), typeof(Resources))]
        [LocalizableDescription(nameof(Resources.Success_Description), typeof(Resources))]
        [IsOut]
        public bool Success { get; set; }

        public override void Execute(int? optionID)
        {
            try
            {
                switch (OutputFormat.ToLower())
                {
                    case "json":
                        ConvertCsvToJson();
                        break;
                    case "xml":
                        ConvertCsvToXml();
                        break;
                    default:
                        throw new ArgumentException("Unsupported output format.");
                }

                Success = true;
            }
            catch (Exception ex)
            {
                Success = false;
                throw new Exception($"Error during conversion: {ex.Message}", ex);
            }
        }

        private void ConvertCsvToJson()
        {
            var csvContent = File.ReadAllText(InputCsvFile, GetEncoding(EncodingType));
            var lines = csvContent.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            var headers = lines[0].Split(GetDelimiterChar(CsvDelimiter));

            var jsonArray = new JArray(
                lines.Skip(1).Select(line =>
                {
                    var values = line.Split(GetDelimiterChar(CsvDelimiter));
                    var obj = new JObject();
                    for (int i = 0; i < headers.Length && i < values.Length; i++)
                    {
                        // Используйте имена заголовков из CSV в качестве ключей для объекта JSON
                        obj[headers[i]] = values[i];
                    }
                    return obj;
                })
            );

            // Записываем массив объектов JSON в файл
            File.WriteAllText(OutputConvertedFile, jsonArray.ToString(), GetEncoding(EncodingType));
        }
        private void ConvertCsvToXml()
        {
            var csvContent = File.ReadAllText(InputCsvFile, GetEncoding(EncodingType));
            var lines = csvContent.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            var headers = lines[0].Split(GetDelimiterChar(CsvDelimiter));

            var xmlDocument = new XDocument(new XElement("Data",
                lines.Skip(1).Select(line =>
                {
                    var values = line.Split(GetDelimiterChar(CsvDelimiter));
                    return new XElement("Row",
                        headers.Zip(values, (header, value) => new XElement(header, value)));
                })
            ));

            // Сохраняем XML документ в файл
            using (var xmlWriter = XmlWriter.Create(OutputConvertedFile))
            {
                xmlDocument.Save(xmlWriter);
            }
        }

        // Метод для получения символа-разделителя на основе выбранного типа разделителя
        private char GetDelimiterChar(TypeDelimiter delimiterType)
        {
            switch (delimiterType)
            {
                case TypeDelimiter.Comma:
                    return ',';
                case TypeDelimiter.Semicolon:
                    return ';';
                case TypeDelimiter.Tab:
                    return '\t';
                case TypeDelimiter.Caret:
                    return '^';
                case TypeDelimiter.Pipe:
                    return '|';
                default:
                    throw new ArgumentException("Unsupported delimiter type.");
            }
        }

        // Метод для получения объекта кодировки на основе выбранного типа кодировки
        private Encoding GetEncoding(TypeEncoding encodingType)
        {
            switch (encodingType)
            {
                case TypeEncoding.ASCII:
                    return Encoding.ASCII;
                case TypeEncoding.UTF7:
                    return Encoding.UTF7;
                case TypeEncoding.UTF8:
                    return Encoding.UTF8;
                case TypeEncoding.UTF32:
                    return Encoding.UTF32;
                case TypeEncoding.Default:
                    return Encoding.Default;
                case TypeEncoding.ANSI:
                    return Encoding.Default; // ANSI соответствует Default
                case TypeEncoding.Windows1251:
                    return Encoding.GetEncoding(1251); // Кодировка Windows-1251
                default:
                    throw new ArgumentException("Unsupported encoding type.");
            }
        }
    }
}