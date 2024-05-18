using BR.Core.Attributes;
using Activities.Utilities.Properties;
using Newtonsoft.Json.Linq;

namespace Activities.Utilities;

// �����, �������������� ���������� ��� ������ JSON-����� � �������������� ��� ����������� � �������
[LocalizableScreenName(nameof(Resources.JsonDictionary_ScreenName), typeof(Resources))]
[BR.Core.Attributes.Path("Utilities")]
public class JsonDictionary : BR.Core.Activity
{
    // ���� � ����� JSON, ������� ����� ��������� � �������������
    [LocalizableScreenName(nameof(Resources.Patch_ScreenName), typeof(Resources))]
    [LocalizableDescription(nameof(Resources.Patch_Description), typeof(Resources))]
    [IsRequired]
    public BR.Core.FilePath Patch { get; set; }

    // �������������� �������, ���� ����� �������� ������ �� JSON
    [LocalizableScreenName(nameof(Resources.Out_Dict_ScreenName), typeof(Resources))]
    [LocalizableDescription(nameof(Resources.Out_Dict_Description), typeof(Resources))]
    [IsOut]
    public Dictionary<string, object> Dict_Out { get; set; }

    // �����, ����������� ���������� ������ JSON � �������������� � �������
    public override void Execute(int? optionID)
    {
        // �������� ������� ���� � ����� JSON
        if (Patch == null)
        {
            throw new ArgumentNullException(nameof(Patch), "Patch file path is not provided.");
        }

        try
        {
            // ������ ����������� JSON-�����
            string jsonText = File.ReadAllText(Patch);

            // �������������� JSON � �������
            Dict_Out = DeserializeJsonToDictionary(jsonText);
        }
        catch (Exception ex)
        {
            throw new Exception("Error occurred while parsing JSON file.", ex);
        }
    }

    // ��������� ������� ��� �������������� JSON � �������
    private Dictionary<string, object> DeserializeJsonToDictionary(string jsonText, string prefix = "")
    {
        var result = new Dictionary<string, object>();
        var jObject = JObject.Parse(jsonText);

        // �������� �� ��������� JSON-�������
        foreach (var property in jObject.Properties())
        {
            // ������������ ����� ��� ���������� � �������
            var key = string.IsNullOrEmpty(prefix) ? property.Name : $"{prefix}.{property.Name}";

            // ���� �������� - ������, ���������� �������� DeserializeJsonToDictionary ��� ���������� �������
            if (property.Value.Type == JTokenType.Object)
            {
                var subDictionary = DeserializeJsonToDictionary(property.Value.ToString(), key);
                foreach (var kvp in subDictionary)
                {
                    result.Add(kvp.Key, kvp.Value);
                }
            }
            // ���� �������� - ������, ������������ ������ ������� �������
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
            // � ��������� ������ ��������� �������� �������� � �������
            else
            {
                result.Add(key, ((JValue)property.Value).Value);
            }
        }

        return result;
    }
}
