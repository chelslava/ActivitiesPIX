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
John;12;Уфа
Alice;15;Краснодар
Bob;40;Москва
```

### JSON файл (результат конвертации)

```json
[
  {
    "Name": "John",
    "Age": "12",
    "City": "Уфа"
  },
  {
    "Name": "Alice",
    "Age": "15",
    "City": "Краснодар"
  },
  {
    "Name": "Bob",
    "Age": "40",
    "City": "Москва"
  }
]
```
```xml
<?xml version="1.0" encoding="utf-8"?>
<Data>
	<Row>
		<Name>John</Name>
		<Age>12</Age>
		<City>Уфа</City>
	</Row>
	<Row>
		<Name>Alice</Name>
		<Age>15</Age>
		<City>Краснодар</City>
	</Row>
	<Row>
		<Name>Bob</Name>
		<Age>40</Age>
		<City>Москва</City>
	</Row>
</Data>
```
