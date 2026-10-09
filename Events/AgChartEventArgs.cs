using System.Text.Json;

namespace RazorAgCharts;

/// <summary>Click or double-click payload forwarded from an AG Charts listener.</summary>
public sealed class AgChartEventArgs
{
    public string? Type { get; init; }

    public string? SeriesId { get; init; }

    public string? ItemId { get; init; }

    public string? XKey { get; init; }

    public string? YKey { get; init; }

    public string? AngleKey { get; init; }

    public string? RadiusKey { get; init; }

    public string? SizeKey { get; init; }

    public string? LabelKey { get; init; }

    public string? CalloutLabelKey { get; init; }

    public string? SectorLabelKey { get; init; }

    public JsonElement Datum { get; init; }

    public JsonElement Coordinates { get; init; }

    public JsonElement Raw { get; init; }

    public T? GetDatum<T>()
    {
        if (Datum.ValueKind is JsonValueKind.Undefined or JsonValueKind.Null)
        {
            return default;
        }

        return Datum.Deserialize<T>(AgChartsJson.SerializerOptions);
    }

    internal static AgChartEventArgs From(JsonElement raw)
    {
        return new AgChartEventArgs
        {
            Type = GetString(raw, "type"),
            SeriesId = GetString(raw, "seriesId"),
            ItemId = GetString(raw, "itemId"),
            XKey = GetString(raw, "xKey"),
            YKey = GetString(raw, "yKey"),
            AngleKey = GetString(raw, "angleKey"),
            RadiusKey = GetString(raw, "radiusKey"),
            SizeKey = GetString(raw, "sizeKey"),
            LabelKey = GetString(raw, "labelKey"),
            CalloutLabelKey = GetString(raw, "calloutLabelKey"),
            SectorLabelKey = GetString(raw, "sectorLabelKey"),
            Datum = CloneProperty(raw, "datum"),
            Coordinates = CloneProperty(raw, "coordinates"),
            Raw = raw.Clone(),
        };
    }

    private static string? GetString(JsonElement element, string name)
    {
        if (!element.TryGetProperty(name, out var value) || value.ValueKind != JsonValueKind.String)
        {
            return null;
        }

        return value.GetString();
    }

    private static JsonElement CloneProperty(JsonElement element, string name)
    {
        return element.TryGetProperty(name, out var value) ? value.Clone() : default;
    }
}
