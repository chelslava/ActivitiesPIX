using Activities.Utilites.Properties;
using BR.Core;
using BR.Core.Attributes;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Namespace_Utilites
{
    [LocalizableScreenName("CSVConverter_ScreenName", typeof(Resources))]
    [BR.Core.Attributes.Path("Utilites")]
    public class CSVConverter : Activity
    {
        [LocalizableScreenName("InputCsvFile_ScreenName", typeof(Resources))]
        [LocalizableDescription("OutputConvertedFile_ScreenName", typeof(Resources))]
        [IsRequired]
        [IsFilePathChooser]
        public FilePath InputCsvFile { get; set; }

        [LocalizableScreenName("OutputConvertedFile_ScreenName", typeof(Resources))]
        [LocalizableDescription("OutputConvertedFile_Description", typeof(Resources))]
        [IsRequired]
        [IsFilePathChooser]
        public FilePath OutputConvertedFile { get; set; }

        [LocalizableScreenName("OutputFormat_ScreenName", typeof(Resources))]
        [LocalizableDescription("OutputFormat_Description", typeof(Resources))]
        [IsRequired]
        public OutputFormat OutputFormat { get; set; }

        [LocalizableScreenName("CsvDelimiter_ScreenName", typeof(Resources))]
        [LocalizableDescription("CsvDelimiter_Description", typeof(Resources))]
        [IsRequired]
        public TypeDelimiter CsvDelimiter { get; set; }

        [LocalizableScreenName("EncodingType_ScreenName", typeof(Resources))]
        [LocalizableDescription("EncodingType_Description", typeof(Resources))]
        [IsRequired]
        public TypeEncoding EncodingType { get; set; }

        [LocalizableScreenName("Success_ScreenName", typeof(Resources))]
        [LocalizableDescription("Success_Description", typeof(Resources))]
        [IsOut]
        public bool Success { get; set; }

        public override void Execute(int? optionId)
        {
            try
            {
                // Проверяем формат выходного файла и вызываем соответствующий метод конвертации
                if (OutputFormat == OutputFormat.JSON)
                {
                    ConvertCsvToJson();
                }
                else if (OutputFormat == OutputFormat.XML)
                {
                    ConvertCsvToXml();
                }
                else
                {
                    throw new ArgumentException("Unsupported output format.");
                }
                Success = true; // Конвертация завершена успешно
            }
            catch (Exception ex)
            {
                Success = false; // Произошла ошибка при конвертации
                throw new Exception($"Error during conversion: {ex.Message}", ex);
            }
        }
        private void ConvertCsvToJson()
        {
            // Чтение данных из CSV файла и преобразование в JSON
            string[] lines = File.ReadAllLines(InputCsvFile, GetEncoding(EncodingType));
            string[] headers = lines[0].Split(GetDelimiterChar(CsvDelimiter));

            var data = lines.Skip(1)
                            .Select(line =>
                            {
                                string[] values = line.Split(GetDelimiterChar(CsvDelimiter));
                                return headers.Zip(values, (header, value) => new { Header = header, Value = value })
                                              .ToDictionary(pair => pair.Header, pair => (object)pair.Value);
                            });

            // Создание и сохранение JSON массива в выходной файл
            JArray jsonArray = new JArray(data.Select(line =>
            {
                JObject jsonObject = new JObject();
                foreach (var pair in line)
                {
                    jsonObject[pair.Key] = new JValue(pair.Value);
                }
                return jsonObject;
            }));
            File.WriteAllText(OutputConvertedFile, jsonArray.ToString(), GetEncoding(EncodingType));
        }
        private void ConvertCsvToXml()
        {
            string[] lines = File.ReadAllLines(InputCsvFile, GetEncoding(EncodingType));

            string[] headers = lines[0].Split(GetDelimiterChar(CsvDelimiter));

            var xmlData = lines
                .Skip(1)
                .Select(line =>
                {
                    string[] values = line.Split(GetDelimiterChar(CsvDelimiter));
                    return new XElement("Row", headers.Zip(values, (header, value) => new XElement(header, value)));
                });

            XDocument xmlDocument = new XDocument(new XElement("Data", xmlData));

            using (XmlWriter xmlWriter = XmlWriter.Create(OutputConvertedFile))
            {
                xmlDocument.Save(xmlWriter);
            }
        }

        private char GetDelimiterChar(TypeDelimiter delimiterType)
        {
            return delimiterType switch
            {
                TypeDelimiter.Comma => ',',
                TypeDelimiter.Semicolon => ';',
                TypeDelimiter.Tab => '\t',
                TypeDelimiter.Caret => '^',
                TypeDelimiter.Pipe => '|',
                _ => throw new ArgumentException("Unsupported delimiter type.")
            };
        }

        private Encoding GetEncoding(TypeEncoding encodingType)
        {
            return encodingType switch
            {
                TypeEncoding.ASCII => Encoding.ASCII,
                TypeEncoding.UTF7 => Encoding.UTF7,
                TypeEncoding.UTF8 => Encoding.UTF8,
                TypeEncoding.UTF32 => Encoding.UTF32,
                TypeEncoding.Default => Encoding.Default,
                TypeEncoding.ANSI => Encoding.Default,
                TypeEncoding.Windows1251 => Encoding.GetEncoding(1251),
                _ => throw new ArgumentException("Unsupported encoding type.")
            };
        }
    }
}
