using System.Text.Json;
using System.Text.Json.Serialization;

namespace RazorAgCharts;

/// <summary>
/// A JavaScript function expression passed through to AG Charts for formatters, renderers, and listeners.
/// AG Charts calls these in the browser. They are not C# delegates.
/// </summary>
/// <remarks>
/// The expression must be a function, for example <c>(params) =&gt; params.value + ' °C'</c>
/// or <c>function (params) { return params.value; }</c>.
/// Only pass expressions you trust. They are compiled in the browser.
/// </remarks>
public sealed class JsCallback
{
    public JsCallback(string functionExpression)
    {
        if (string.IsNullOrWhiteSpace(functionExpression))
        {
            throw new ArgumentException("A JavaScript function expression is required.", nameof(functionExpression));
        }

        FunctionExpression = functionExpression;
    }

    public string FunctionExpression { get; }

    public static JsCallback Arrow(string functionExpression) => new(functionExpression);
}

internal sealed class JsCallbackConverter : JsonConverter<JsCallback>
{
    public const string PropertyName = "$js";

    public override JsCallback? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        using var document = JsonDocument.ParseValue(ref reader);
        if (!document.RootElement.TryGetProperty(PropertyName, out var source)
            || source.ValueKind != JsonValueKind.String)
        {
            throw new JsonException("A JsCallback must be an object with a \"$js\" function expression.");
        }

        return new JsCallback(source.GetString()!);
    }

    public override void Write(Utf8JsonWriter writer, JsCallback value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteString(PropertyName, value.FunctionExpression);
        writer.WriteEndObject();
    }
}
