using System;
using System.Text.Json;
using System.Collections.Generic;

// Компонент (Component) — базовий інтерфейс
interface IJsonData
{
string GetContent();
}

// Конкретний компонент (ConcreteComponent)
class JsonData : IJsonData
{
private string _json;

```
public JsonData(string json)
{
    _json = json;
}

public string GetContent()
{
    return _json;
}
```

}

// Базовий декоратор (Decorator)
abstract class JsonDecorator : IJsonData
{
protected IJsonData _jsonData;

```
protected JsonDecorator(IJsonData jsonData)
{
    _jsonData = jsonData;
}

public virtual string GetContent()
{
    return _jsonData.GetContent();
}
```

}

// Конкретний декоратор для HTML-таблиці
class HtmlTableDecorator : JsonDecorator
{
public HtmlTableDecorator(IJsonData jsonData) : base(jsonData) { }

```
public override string GetContent()
{
    string json = _jsonData.GetContent();
    var items = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(json);
    if (items == null || items.Count == 0)
        return "<p>No data</p>";

    var html = "<table border='1'><thead><tr>";
    foreach (var header in items[0].Keys)
    {
        html += $"<th>{header}</th>";
    }
    html += "</tr></thead><tbody>";

    foreach (var item in items)
    {
        html += "<tr>";
        foreach (var value in item.Values)
        {
            html += $"<td>{value}</td>";
        }
        html += "</tr>";
    }
    html += "</tbody></table>";
    return html;
}
```

}

// Додатковий декоратор для заголовку
class HtmlHeaderDecorator : JsonDecorator
{
private string _title;

```
public HtmlHeaderDecorator(IJsonData jsonData, string title) : base(jsonData)
{
    _title = title;
}

public override string GetContent()
{
    return $"<h2>{_title}</h2>{_jsonData.GetContent()}";
}
```

}

class Program
{
static void Main()
{
string json = @"
[
{ ""Name"": ""Alice"", ""Age"": 30, ""City"": ""Kyiv"" },
{ ""Name"": ""Bob"", ""Age"": 25, ""City"": ""Lviv"" }
]";

```
    IJsonData data = new JsonData(json);
    IJsonData table = new HtmlTableDecorator(data);
    IJsonData headerAndTable = new HtmlHeaderDecorator(table, "User Info");

    Console.WriteLine(headerAndTable.GetContent());
}
```

}
