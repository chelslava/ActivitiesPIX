# Парсер JSON в Словарь

Данная программа представляет собой активность PIX Studio, разработанную для чтения JSON-файлов и преобразования их содержимого в формат словаря. Основная цель - обеспечить удобный способ работы с данными в формате JSON, преобразуя их в структуру пар ключ-значение.

## Пример использования:

**JSON-данные**
```json
{
    "name": "John Doe",
    "age": 30,
    "isStudent": false,
    "address": {
        "street": "123 Main St",
        "city": "Anytown",
        "zip": "12345"
    },
    "skills": ["programming", "design", "writing"]
}
```

**Ожидаемый словарь на выходе**
```
{
    { "name", "John Doe" },
    { "age", 30 },
    { "isStudent", false },
    { "address.street", "123 Main St" },
    { "address.city", "Anytown" },
    { "address.zip", "12345" },
    { "skills", new List<string> { "programming", "design", "writing" } }
}
```

# CSV Converter

Активность для конвертации данных из формата CSV в другие форматы данных (JSON, XML).

## Входные данные

- **Входной CSV файл**: Путь к файлу данных в формате CSV, который требуется сконвертировать.
- **Выходной файл**: Путь к файлу, в который будет сохранен результат конвертации.
- **Формат вывода**: Формат, в который будет производиться конвертация (JSON, XML).
- **Разделитель CSV**: Тип разделителя в CSV файле (запятая, точка с запятой, табуляция и т. д.).
- **Тип кодировки**: Тип кодировки файла (ASCII, UTF-8, ANSI и т. д.).

## Примеры входных и выходных данных

### CSV файл
```
Name;Age;City
John;30;New York
Alice;25;Los Angeles
Bob;35;Chicago
```

### JSON файл (результат конвертации)

```json
[
{
        "Name": "John",
        "Age": 30,
        "City": "New York"
    }, {
        "Name": "Alice",
        "Age": 25,
        "City": "Los Angeles"
    }, {
        "Name": "Bob",
        "Age": 35,
        "City": "Chicago"
    }
]
```
```xml
<Data>
  <Row>
    <Name>John</Name>
    <Age>30</Age>
    <City>New York</City>
  </Row>
  <Row>
    <Name>Alice</Name>
    <Age>25</Age>
    <City>Los Angeles</City>
  </Row>
  <Row>
    <Name>Bob</Name>
    <Age>35</Age>
    <City>Chicago</City>
  </Row>
</Data>
```
