namespace RazorAgCharts;

/// <summary>Title, subtitle, or footnote text.</summary>
public sealed class AgCaptionOptions
{
    public bool? Enabled { get; set; }

    public string? Text { get; set; }

    public string? TextAlign { get; set; }

    public double? FontSize { get; set; }

    public string? FontFamily { get; set; }

    public object? FontWeight { get; set; }

    public string? FontStyle { get; set; }

    public string? Color { get; set; }

    public double? Spacing { get; set; }

    public string? Wrapping { get; set; }
}

/// <summary>Padding in pixels. A value on every side matches a uniform AG Charts padding object.</summary>
public sealed class AgPaddingOptions
{
    public double? Top { get; set; }

    public double? Right { get; set; }

    public double? Bottom { get; set; }

    public double? Left { get; set; }

    public static AgPaddingOptions Uniform(double value) => new()
    {
        Top = value,
        Right = value,
        Bottom = value,
        Left = value,
    };
}

public sealed class AgBackgroundOptions
{
    public bool? Visible { get; set; }

    public string? Fill { get; set; }
}

public sealed class AgHighlightOptions
{
    public bool? Enabled { get; set; }

    /// <summary><c>tooltip</c> or <c>node</c>.</summary>
    public string? Range { get; set; }

    /// <summary><c>single</c> or <c>shared</c>.</summary>
    public string? Mode { get; set; }

    /// <summary><c>overlay</c> or <c>cutout</c>.</summary>
    public string? DrawingMode { get; set; }
}

public sealed class AgAnimationOptions
{
    public bool? Enabled { get; set; }

    public double? Duration { get; set; }
}

public sealed class AgZoomOptions
{
    public bool? Enabled { get; set; }

    public bool? EnableAxisDragging { get; set; }

    public bool? EnablePanning { get; set; }

    public bool? EnableScrolling { get; set; }

    public bool? EnableSelecting { get; set; }

    public double? ScrollingStep { get; set; }

    /// <summary><c>x</c>, <c>y</c>, or <c>xy</c>.</summary>
    public string? Axes { get; set; }
}

public sealed class AgMarkerOptions
{
    public bool? Enabled { get; set; }

    public double? Size { get; set; }

    public string? Shape { get; set; }

    public string? Fill { get; set; }

    public string? Stroke { get; set; }

    public double? StrokeWidth { get; set; }
}

public sealed class AgSeriesLabelOptions
{
    public bool? Enabled { get; set; }

    public JsCallback? Formatter { get; set; }

    public string? Color { get; set; }

    public double? FontSize { get; set; }

    public string? FontFamily { get; set; }

    public string? Placement { get; set; }
}

public sealed class AgTooltipOptions
{
    public bool? Enabled { get; set; }

    /// <summary><c>single</c>, <c>shared</c>, or <c>compact</c>.</summary>
    public string? Mode { get; set; }

    /// <summary>A pixel number, or <c>exact</c> / <c>nearest</c>.</summary>
    public object? Range { get; set; }

    public bool? ShowArrow { get; set; }

    public string? Position { get; set; }

    public JsCallback? Renderer { get; set; }
}

public sealed class AgLegendItemOptions
{
    public AgMarkerOptions? Marker { get; set; }

    public AgSeriesLabelOptions? Label { get; set; }

    public double? PaddingX { get; set; }

    public double? PaddingY { get; set; }

    public double? MaxWidth { get; set; }
}

public sealed class AgLegendOptions
{
    public bool? Enabled { get; set; }

    public string? Position { get; set; }

    public double? Spacing { get; set; }

    public bool? ReverseOrder { get; set; }

    public bool? ToggleSeries { get; set; }

    public double? MaxWidth { get; set; }

    public double? MaxHeight { get; set; }

    public AgLegendItemOptions? Item { get; set; }
}

/// <summary>
/// Chart-level JavaScript listeners. C# handlers on <see cref="AgChart"/> are separate and can be used together with these.
/// </summary>
public sealed class AgChartListeners
{
    public JsCallback? Click { get; set; }

    public JsCallback? DoubleClick { get; set; }

    public JsCallback? SeriesNodeClick { get; set; }

    public JsCallback? SeriesNodeDoubleClick { get; set; }
}

public sealed class AgSeriesListeners
{
    public JsCallback? SeriesNodeClick { get; set; }

    public JsCallback? SeriesNodeDoubleClick { get; set; }
}

/// <summary>Theme object layered on a built-in theme. Assign a theme name string to <see cref="AgChartOptions.Theme"/> for a preset.</summary>
public sealed class AgChartTheme
{
    public string? BaseTheme { get; set; }

    public object? Palette { get; set; }

    public object? Params { get; set; }

    public object? Overrides { get; set; }
}
