# Парсер JSON в Словарь

Данная программа представляет собой активность PIX Studio, разработанную для чтения JSON-файлов и преобразования их содержимого в формат словаря. Основная цель - обеспечить удобный способ работы с данными в формате JSON, преобразуя их в структуру пар ключ-значение.

## Пример использования:

// JSON-данные
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

// Ожидаемый словарь на выходе
Dictionary<string, object> ожидаемыйСловарь = new Dictionary<string, object>
{
    { "name", "John Doe" },
    { "age", 30 },
    { "isStudent", false },
    { "address.street", "123 Main St" },
    { "address.city", "Anytown" },
    { "address.zip", "12345" },
    { "skills", new List<string> { "programming", "design", "writing" } }
};
