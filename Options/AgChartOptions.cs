using System.Text.Json.Nodes;

namespace RazorAgCharts;

/// <summary>
/// Options passed to <c>AgCharts.create</c> or <c>update</c>.
/// Property names are serialized in camelCase so they match the JavaScript options reference.
/// Data property names are camelCased the same way: a C# property <c>Month</c> is sent as <c>month</c>,
/// so series keys should use that serialized name.
/// </summary>
/// <remarks>
/// <see cref="Additional"/> is merged onto the serialized object. Use it for any AG Charts option this type does not declare,
/// including Enterprise configuration. Keys are written exactly as supplied.
/// </remarks>
public sealed class AgChartOptions
{
    public object? Data { get; set; }

    public string? DataIdKey { get; set; }

    public IList<AgSeriesOptions>? Series { get; set; }

    /// <summary>Axes keyed by id. Cartesian charts use <c>x</c> and <c>y</c> unless a series points at another key.</summary>
    public IDictionary<string, AgAxisOptions>? Axes { get; set; }

    public AgCaptionOptions? Title { get; set; }

    public AgCaptionOptions? Subtitle { get; set; }

    public AgCaptionOptions? Footnote { get; set; }

    public AgLegendOptions? Legend { get; set; }

    public AgTooltipOptions? Tooltip { get; set; }

    /// <summary>A theme name such as <see cref="AgChartThemes.Default"/>, or an <see cref="AgChartTheme"/> object.</summary>
    public object? Theme { get; set; }

    public AgPaddingOptions? Padding { get; set; }

    public AgBackgroundOptions? Background { get; set; }

    public AgHighlightOptions? Highlight { get; set; }

    public AgAnimationOptions? Animation { get; set; }

    public AgZoomOptions? Zoom { get; set; }

    public object? Context { get; set; }

    public object? Locale { get; set; }

    public bool? Loading { get; set; }

    public double? Width { get; set; }

    public double? Height { get; set; }

    public double? MinWidth { get; set; }

    public double? MinHeight { get; set; }

    public bool? EnableRtl { get; set; }

    public string? StyleNonce { get; set; }

    public bool? LoadGoogleFonts { get; set; }

    public bool? SuppressFieldDotNotation { get; set; }

    public AgChartListeners? Listeners { get; set; }

    public JsonNode? Navigator { get; set; }

    public JsonNode? ContextMenu { get; set; }

    public JsonNode? InitialState { get; set; }

    public JsonNode? DataSource { get; set; }

    public JsonNode? Sync { get; set; }

    public JsonNode? Overlays { get; set; }

    public JsonNode? Formatter { get; set; }

    public IDictionary<string, object?>? Additional { get; set; }
}

/// <summary>Partial data change passed to <c>AgChartInstance.applyTransaction</c>.</summary>
public sealed class AgDataTransaction
{
    public IEnumerable<object>? Add { get; set; }

    public IEnumerable<object>? Remove { get; set; }

    public IEnumerable<object>? Update { get; set; }

    public int? AddIndex { get; set; }
}

/// <summary>Image size passed to <c>download</c> and <c>getImageDataURL</c>.</summary>
public sealed class AgChartImageOptions
{
    public string? FileName { get; set; }

    public double? Width { get; set; }

    public double? Height { get; set; }
}
