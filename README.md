**������ JSON � �������**

������ ��������� ������������ ����� ���������� PIX Studio, ������������� ��� ������ JSON-������ � �������������� �� ����������� � ������ �������. �������� ���� - ���������� ������� ������ ������ � ������� � ������� JSON, ���������� �� � ��������� ��� ����-��������.

**������ �������������:**

// JSON-������
string json = @"
{
    ""name"": ""John Doe"",
    ""age"": 30,
    ""isStudent"": false,
    ""address"": {
        ""street"": ""123 Main St"",
        ""city"": ""Anytown"",
        ""zip"": ""12345""
    },
    ""skills"": [""programming"", ""design"", ""writing""]
}";

// ��������� ������� �� ������
Dictionary<string, object> ���������������� = new Dictionary<string, object>
{
    { "name", "John Doe" },
    { "age", 30 },
    { "isStudent", false },
    { "address.street", "123 Main St" },
    { "address.city", "Anytown" },
    { "address.zip", "12345" },
    { "skills", new List<string> { "programming", "design", "writing" } }
};
