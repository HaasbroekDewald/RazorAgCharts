using RazorAgCharts;

namespace RazorAgCharts.Sample;

public sealed record IceCreamMonth(string Month, double AvgTemp, double IceCreamSales);

public static class SampleCharts
{
    public static IReadOnlyList<IceCreamMonth> Months { get; } =
    [
        new("Jan", 2.3, 162000),
        new("Mar", 6.3, 302000),
        new("May", 16.2, 800000),
        new("Jul", 22.8, 1254000),
        new("Sep", 14.5, 950000),
        new("Nov", 8.9, 200000),
    ];

    public static AgChartOptions IceCream(IEnumerable<IceCreamMonth>? data = null, string? theme = null)
    {
        var line = AgSeriesOptions.Line("month", "avgTemp", "Average Temperature (°C)");
        line.YKeyAxis = "temperature";

        var bars = AgSeriesOptions.Bar("month", "iceCreamSales", "Ice Cream Sales");
        bars.YKeyAxis = "sales";

        return new AgChartOptions
        {
            Theme = theme,
            Title = new AgCaptionOptions { Text = "Ice Cream Sales and Average Temperature" },
            Subtitle = new AgCaptionOptions { Text = "Data from 2022" },
            Data = (data ?? Months).ToList(),
            Series = [bars, line],
            Axes = new Dictionary<string, AgAxisOptions>
            {
                ["x"] = new()
                {
                    Type = AgAxisTypes.Category,
                    Position = AgAxisPositions.Bottom,
                },
                ["sales"] = new()
                {
                    Type = AgAxisTypes.Number,
                    Position = AgAxisPositions.Left,
                    Title = new AgAxisTitleOptions { Text = "Sales" },
                    Label = new AgAxisLabelOptions
                    {
                        Formatter = new JsCallback("(params) => Number(params.value).toLocaleString()"),
                    },
                },
                ["temperature"] = new()
                {
                    Type = AgAxisTypes.Number,
                    Position = AgAxisPositions.Right,
                    Title = new AgAxisTitleOptions { Text = "Temperature" },
                    Label = new AgAxisLabelOptions
                    {
                        Formatter = new JsCallback("(params) => params.value + ' °C'"),
                    },
                },
            },
            Legend = new AgLegendOptions { Position = AgLegendPositions.Bottom },
        };
    }
}
