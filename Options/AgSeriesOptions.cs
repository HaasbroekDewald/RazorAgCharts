namespace RazorAgCharts;

/// <summary>
/// One AG Charts series. Use <see cref="Additional"/> for options that this type does not declare.
/// Keys in <see cref="Additional"/> are written as given, so use the JavaScript names from the AG Charts options reference.
/// </summary>
public sealed class AgSeriesOptions
{
    public string? Type { get; set; }

    public string? Id { get; set; }

    public object? Data { get; set; }

    public bool? Visible { get; set; }

    public string? XKey { get; set; }

    public string? XName { get; set; }

    public string? YKey { get; set; }

    public string? YName { get; set; }

    public IList<string>? YKeys { get; set; }

    public string? XKeyAxis { get; set; }

    public string? YKeyAxis { get; set; }

    public string? AngleKey { get; set; }

    public string? AngleName { get; set; }

    public string? RadiusKey { get; set; }

    public string? RadiusName { get; set; }

    public string? SizeKey { get; set; }

    public string? SizeName { get; set; }

    public string? LabelKey { get; set; }

    public string? LabelName { get; set; }

    public string? LegendItemKey { get; set; }

    public string? CalloutLabelKey { get; set; }

    public string? SectorLabelKey { get; set; }

    /// <summary><see cref="AgBarDirections"/>.</summary>
    public string? Direction { get; set; }

    public bool? Stacked { get; set; }

    public string? StackGroup { get; set; }

    public double? NormalizedTo { get; set; }

    public string? Fill { get; set; }

    public double? FillOpacity { get; set; }

    public string? Stroke { get; set; }

    public double? StrokeWidth { get; set; }

    public double? StrokeOpacity { get; set; }

    public double? CornerRadius { get; set; }

    public double? InnerRadiusRatio { get; set; }

    /// <summary><see cref="AgLineInterpolations"/>.</summary>
    public string? Interpolation { get; set; }

    /// <summary>A pixel number, or <c>exact</c> / <c>nearest</c>.</summary>
    public object? NodeClickRange { get; set; }

    public AgMarkerOptions? Marker { get; set; }

    public AgSeriesLabelOptions? Label { get; set; }

    public AgSeriesLabelOptions? CalloutLabel { get; set; }

    public AgSeriesLabelOptions? SectorLabel { get; set; }

    public AgTooltipOptions? Tooltip { get; set; }

    public AgSeriesListeners? Listeners { get; set; }

    public IDictionary<string, object?>? Additional { get; set; }

    public static AgSeriesOptions Bar(string xKey, string yKey, string? yName = null) => new()
    {
        Type = AgChartSeriesTypes.Bar,
        XKey = xKey,
        YKey = yKey,
        YName = yName,
    };

    public static AgSeriesOptions Line(string xKey, string yKey, string? yName = null) => new()
    {
        Type = AgChartSeriesTypes.Line,
        XKey = xKey,
        YKey = yKey,
        YName = yName,
    };

    public static AgSeriesOptions Area(string xKey, string yKey, string? yName = null) => new()
    {
        Type = AgChartSeriesTypes.Area,
        XKey = xKey,
        YKey = yKey,
        YName = yName,
    };

    public static AgSeriesOptions Scatter(string xKey, string yKey, string? yName = null) => new()
    {
        Type = AgChartSeriesTypes.Scatter,
        XKey = xKey,
        YKey = yKey,
        YName = yName,
    };

    public static AgSeriesOptions Bubble(string xKey, string yKey, string sizeKey, string? yName = null) => new()
    {
        Type = AgChartSeriesTypes.Bubble,
        XKey = xKey,
        YKey = yKey,
        SizeKey = sizeKey,
        YName = yName,
    };

    public static AgSeriesOptions Histogram(string xKey) => new()
    {
        Type = AgChartSeriesTypes.Histogram,
        XKey = xKey,
    };

    public static AgSeriesOptions Pie(string angleKey, string? calloutLabelKey = null) => new()
    {
        Type = AgChartSeriesTypes.Pie,
        AngleKey = angleKey,
        CalloutLabelKey = calloutLabelKey,
    };

    public static AgSeriesOptions Donut(string angleKey, string? calloutLabelKey = null, double innerRadiusRatio = 0.6) => new()
    {
        Type = AgChartSeriesTypes.Donut,
        AngleKey = angleKey,
        CalloutLabelKey = calloutLabelKey,
        InnerRadiusRatio = innerRadiusRatio,
    };
}
