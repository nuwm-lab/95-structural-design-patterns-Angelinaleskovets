using System;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Collections.Generic;

namespace JsonToHtmlDecorator
{
// Компонент (Component)
public interface IJsonData
{
string GetContent();
}

```
// Конкретний компонент (ConcreteComponent)
public class JsonData : IJsonData
{
    private readonly string _json;

    public JsonData(string json)
    {
        _json = json;
    }

    public string GetContent()
    {
        return _json;
    }
}

// Базовий декоратор (Decorator)
public abstract class JsonDecorator : IJsonData
{
    protected readonly IJsonData _jsonData;

    protected JsonDecorator(IJsonData jsonData)
    {
        _jsonData = jsonData ?? throw new ArgumentNullException(nameof(jsonData));
    }

    public virtual string GetContent()
    {
        return _jsonData.GetContent();
    }
}

// Модель для десеріалізації JSON
public class User
{
    public string Name { get; set; } = "";
    public int Age { get; set; }
    public string City { get; set; } = "";
}

// Конкретний декоратор для HTML-таблиці
public class HtmlTableDecorator : JsonDecorator
{
    public HtmlTableDecorator(IJsonData jsonData) : base(jsonData) { }

    public override string GetContent()
    {
        string json = _jsonData.GetContent();
        List<User>? users;
        try
        {
            users = JsonSerializer.Deserialize<List<User>>(json);
        }
        catch (JsonException)
        {
            return "<p>Invalid JSON data</p>";
        }

        if (users == null || users.Count == 0)
            return "<p>No data available</p>";

        var sb = new StringBuilder();
        sb.AppendLine("<table border='1'><thead><tr>");
        sb.AppendLine("<th>Name</th><th>Age</th><th>City</th>");
        sb.AppendLine("</tr></thead><tbody>");

        foreach (var user in users)
        {
            sb.AppendLine("<tr>");
            sb.AppendLine($"<td>{HtmlEncoder.Default.Encode(user.Name)}</td>");
            sb.AppendLine($"<td>{user.Age}</td>");
            sb.AppendLine($"<td>{HtmlEncoder.Default.Encode(user.City)}</td>");
            sb.AppendLine("</tr>");
        }

        sb.AppendLine("</tbody></table>");
        return sb.ToString();
    }
}

// Конкретний декоратор для заголовка
public class HtmlHeaderDecorator : JsonDecorator
{
    private readonly string _title;

    public HtmlHeaderDecorator(IJsonData jsonData, string title) : base(jsonData)
    {
        _title = title;
    }

    public override string GetContent()
    {
        return $"<h2>{HtmlEncoder.Default.Encode(_title)}</h2>{_jsonData.GetContent()}";
    }
}

// Тестування
public class Program
{
    public static void Main()
    {
        string json = @"
        [
            { ""Name"": ""Alice"", ""Age"": 30, ""City"": ""Kyiv"" },
            { ""Name"": ""Bob"", ""Age"": 25, ""City"": ""Lviv"" }
        ]";

        IJsonData data = new JsonData(json);
        IJsonData table = new HtmlTableDecorator(data);
        IJsonData headerAndTable = new HtmlHeaderDecorator(table, "User Info");

        Console.WriteLine(headerAndTable.GetContent());
    }
}
```

}
