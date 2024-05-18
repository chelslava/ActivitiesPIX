using BR.Core.Attributes;
using Activities.Utilities.Properties;
using Newtonsoft.Json.Linq;

namespace Activities.Utilities;

// Класс, представляющий активность для чтения JSON-файла и преобразования его содержимого в словарь
[LocalizableScreenName(nameof(Resources.JsonDictionary_ScreenName), typeof(Resources))]
[BR.Core.Attributes.Path("Utilities")]
public class JsonDictionary : BR.Core.Activity
{
    // Путь к файлу JSON, который нужно прочитать и преобразовать
    [LocalizableScreenName(nameof(Resources.Patch_ScreenName), typeof(Resources))]
    [LocalizableDescription(nameof(Resources.Patch_Description), typeof(Resources))]
    [IsRequired]
    public BR.Core.FilePath Patch { get; set; }

    // Результирующий словарь, куда будут помещены данные из JSON
    [LocalizableScreenName(nameof(Resources.Out_Dict_ScreenName), typeof(Resources))]
    [LocalizableDescription(nameof(Resources.Out_Dict_Description), typeof(Resources))]
    [IsOut]
    public Dictionary<string, object> Dict_Out { get; set; }

    // Метод, выполняющий активность чтения JSON и преобразования в словарь
    public override void Execute(int? optionID)
    {
        // Проверка наличия пути к файлу JSON
        if (Patch == null)
        {
            throw new ArgumentNullException(nameof(Patch), "Patch file path is not provided.");
        }

        try
        {
            // Чтение содержимого JSON-файла
            string jsonText = File.ReadAllText(Patch);

            // Преобразование JSON в словарь
            Dict_Out = DeserializeJsonToDictionary(jsonText);
        }
        catch (Exception ex)
        {
            throw new Exception("Error occurred while parsing JSON file.", ex);
        }
    }

    // Локальная функция для десериализации JSON в словарь
    private Dictionary<string, object> DeserializeJsonToDictionary(string jsonText, string prefix = "")
    {
        var result = new Dictionary<string, object>();
        var jObject = JObject.Parse(jsonText);

        // Итерация по свойствам JSON-объекта
        foreach (var property in jObject.Properties())
        {
            // Формирование ключа для добавления в словарь
            var key = string.IsNullOrEmpty(prefix) ? property.Name : $"{prefix}.{property.Name}";

            // Если свойство - объект, рекурсивно вызываем DeserializeJsonToDictionary для вложенного объекта
            if (property.Value.Type == JTokenType.Object)
            {
                var subDictionary = DeserializeJsonToDictionary(property.Value.ToString(), key);
                foreach (var kvp in subDictionary)
                {
                    result.Add(kvp.Key, kvp.Value);
                }
            }
            // Если свойство - массив, обрабатываем каждый элемент массива
            else if (property.Value.Type == JTokenType.Array)
            {
                var subArray = property.Value as JArray;
                for (int i = 0; i < subArray.Count; i++)
                {
                    var subDictionary = DeserializeJsonToDictionary(subArray[i].ToString(), $"{key}[{i}]");
                    foreach (var kvp in subDictionary)
                    {
                        result.Add(kvp.Key, kvp.Value);
                    }
                }
            }
            // В противном случае добавляем значение свойства в словарь
            else
            {
                result.Add(key, ((JValue)property.Value).Value);
            }
        }

        return result;
    }
}
