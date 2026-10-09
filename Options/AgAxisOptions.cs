namespace RazorAgCharts;

public sealed class AgAxisTitleOptions
{
    public bool? Enabled { get; set; }

    public string? Text { get; set; }

    /// <summary><c>horizontal</c>, <c>vertical</c>, or <c>vertical-reversed</c>.</summary>
    public string? Orientation { get; set; }

    public double? FontSize { get; set; }

    public string? Color { get; set; }

    public double? Spacing { get; set; }
}

public sealed class AgAxisLabelOptions
{
    public bool? Enabled { get; set; }

    public JsCallback? Formatter { get; set; }

    /// <summary>AG Charts number or time format specifier.</summary>
    public string? Format { get; set; }

    public double? FontSize { get; set; }

    public string? Color { get; set; }

    public double? Rotation { get; set; }
}

public sealed class AgGridLineOptions
{
    public bool? Enabled { get; set; }

    public string? Stroke { get; set; }

    public double? Width { get; set; }
}

/// <summary>
/// One axis definition. Place instances in <see cref="AgChartOptions.Axes"/> under keys such as <c>x</c> and <c>y</c>.
/// </summary>
public sealed class AgAxisOptions
{
    public string? Type { get; set; }

    public string? Position { get; set; }

    public AgAxisTitleOptions? Title { get; set; }

    public AgAxisLabelOptions? Label { get; set; }

    public object? Min { get; set; }

    public object? Max { get; set; }

    public bool? Nice { get; set; }

    public bool? Reverse { get; set; }

    public IList<string>? Keys { get; set; }

    public AgGridLineOptions? GridLine { get; set; }

    public IDictionary<string, object?>? Additional { get; set; }
}
