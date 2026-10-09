using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.Unicode;

namespace RazorAgCharts;

/// <summary>Serializes chart options to the JSON document the browser bridge consumes.</summary>
public static class AgChartsJson
{
    public static JsonSerializerOptions SerializerOptions { get; } = CreateOptions();

    public static string Serialize<T>(T value)
    {
        var node = JsonSerializer.SerializeToNode(value, SerializerOptions);
        MergeAdditional(node);
        return node?.ToJsonString(SerializerOptions) ?? "null";
    }

    private static JsonSerializerOptions CreateOptions()
    {
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DictionaryKeyPolicy = null,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
        };
        options.Converters.Add(new JsCallbackConverter());
        return options;
    }

    private static void MergeAdditional(JsonNode? node)
    {
        switch (node)
        {
            case JsonObject obj:
                if (obj.Remove("additional", out var extra) && extra is JsonObject extraObj)
                {
                    foreach (var property in extraObj)
                    {
                        obj[property.Key] = property.Value?.DeepClone();
                    }
                }

                foreach (var property in obj.ToArray())
                {
                    MergeAdditional(property.Value);
                }

                break;
            case JsonArray array:
                foreach (var item in array)
                {
                    MergeAdditional(item);
                }

                break;
        }
    }
}
