namespace RazorAgCharts;

/// <summary>
/// Built-in AG Charts theme names. Financial charts use <see cref="Financial"/> and <see cref="FinancialDark"/>.
/// </summary>
public static class AgChartThemes
{
    public const string Default = "ag-default";
    public const string DefaultDark = "ag-default-dark";
    public const string Sheets = "ag-sheets";
    public const string SheetsDark = "ag-sheets-dark";
    public const string Polychroma = "ag-polychroma";
    public const string PolychromaDark = "ag-polychroma-dark";
    public const string Vivid = "ag-vivid";
    public const string VividDark = "ag-vivid-dark";
    public const string Material = "ag-material";
    public const string MaterialDark = "ag-material-dark";
    public const string Financial = "ag-financial";
    public const string FinancialDark = "ag-financial-dark";
}

/// <summary>Series <c>type</c> values accepted by AG Charts.</summary>
public static class AgChartSeriesTypes
{
    public const string Bar = "bar";
    public const string Line = "line";
    public const string Area = "area";
    public const string Scatter = "scatter";
    public const string Bubble = "bubble";
    public const string Histogram = "histogram";
    public const string Pie = "pie";
    public const string Donut = "donut";

    public const string BoxPlot = "box-plot";
    public const string Candlestick = "candlestick";
    public const string Ohlc = "ohlc";
    public const string Heatmap = "heatmap";
    public const string Waterfall = "waterfall";
    public const string RangeBar = "range-bar";
    public const string RangeArea = "range-area";
    public const string Nightingale = "nightingale";
    public const string RadarLine = "radar-line";
    public const string RadarArea = "radar-area";
    public const string RadialBar = "radial-bar";
    public const string RadialColumn = "radial-column";
    public const string Treemap = "treemap";
    public const string Sunburst = "sunburst";
    public const string Sankey = "sankey";
    public const string Chord = "chord";
    public const string Funnel = "funnel";
    public const string ConeFunnel = "cone-funnel";
    public const string Pyramid = "pyramid";
}

/// <summary>Axis <c>type</c> values. Cartesian charts default to <c>x</c> and <c>y</c> keys.</summary>
public static class AgAxisTypes
{
    public const string Category = "category";
    public const string Number = "number";
    public const string Log = "log";
    public const string Time = "time";
    public const string UnitTime = "unit-time";
    public const string GroupedCategory = "grouped-category";
    public const string AngleCategory = "angle-category";
    public const string AngleNumber = "angle-number";
    public const string RadiusCategory = "radius-category";
    public const string RadiusNumber = "radius-number";
}

/// <summary>Axis <c>position</c> values.</summary>
public static class AgAxisPositions
{
    public const string Top = "top";
    public const string Right = "right";
    public const string Bottom = "bottom";
    public const string Left = "left";
}

/// <summary>Legend <c>position</c> values.</summary>
public static class AgLegendPositions
{
    public const string Top = "top";
    public const string Right = "right";
    public const string Bottom = "bottom";
    public const string Left = "left";
}

/// <summary>Bar series <c>direction</c> values.</summary>
public static class AgBarDirections
{
    public const string Horizontal = "horizontal";
    public const string Vertical = "vertical";
}

/// <summary>Line and area <c>interpolation</c> values.</summary>
public static class AgLineInterpolations
{
    public const string Linear = "linear";
    public const string Smooth = "smooth";
    public const string Step = "step";
}

/// <summary>Which <c>AgCharts</c> factory creates the chart.</summary>
public enum AgChartPreset
{
    /// <summary><c>AgCharts.create</c>. Community and Enterprise.</summary>
    Chart,

    /// <summary><c>AgCharts.createFinancialChart</c>. Requires Enterprise.</summary>
    Financial,

    /// <summary><c>AgCharts.createGauge</c>. Requires Enterprise.</summary>
    Gauge,

    /// <summary><c>AgCharts.createQuadrantChart</c>. Requires Enterprise.</summary>
    Quadrant,
}

/// <summary>AG Charts package to load in the browser.</summary>
public enum AgChartsEdition
{
    Community,
    Enterprise,
}

/// <summary>Package version and CDN script locations used when the host does not supply its own.</summary>
public static class AgChartsDefaults
{
    public const string Version = "14.2.0";

    public static string CommunityScript(string? version = null) =>
        $"https://cdn.jsdelivr.net/npm/ag-charts-community@{version ?? Version}/dist/umd/ag-charts-community.min.js";

    public static string EnterpriseScript(string? version = null) =>
        $"https://cdn.jsdelivr.net/npm/ag-charts-enterprise@{version ?? Version}/dist/umd/ag-charts-enterprise.min.js";
}
