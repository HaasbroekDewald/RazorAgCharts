using RazorAgCharts;

namespace RazorAgCharts.Sample;

public sealed record ChartDemo(string Name, AgChartsEdition Edition, AgChartOptions Options);

public static class ChartGallery
{
    public static IReadOnlyList<ChartDemo> All { get; } =
    [
        Community("Bar", Chart("Quarterly revenue", Quarters(), AgSeriesOptions.Bar("quarter", "revenue", "Revenue"))),
        Community("Line", Chart("Website visits", Months(), AgSeriesOptions.Line("month", "visits", "Visits"))),
        Community("Area", Chart("Monthly visits", Months(), Area())),
        Community("Scatter", Chart("Study time and score", Scores(), AgSeriesOptions.Scatter("hours", "score"))),
        Community("Bubble", Chart("Campaign reach", Campaigns(), AgSeriesOptions.Bubble("spend", "conversions", "reach"))),
        Community("Histogram", Chart("Response times", ResponseTimes(), AgSeriesOptions.Histogram("milliseconds"))),
        Community("Pie", Chart("Browser share", Browsers(), AgSeriesOptions.Pie("share", "browser"), legend: true)),
        Community("Donut", Chart("Support tickets", Tickets(), AgSeriesOptions.Donut("count", "channel"), legend: true)),

        Enterprise("Box plot", Chart("Weekly delivery days", Deliveries(), BoxPlot())),
        Enterprise("Candlestick", Chart("Share price", Prices(), Ohlc(AgChartSeriesTypes.Candlestick))),
        Enterprise("OHLC", Chart("Index movement", Prices(), Ohlc(AgChartSeriesTypes.Ohlc))),
        Enterprise("Heatmap", Chart("Orders by day and hour", Orders(), Heatmap())),
        Enterprise("Waterfall", Chart("Profit bridge", ProfitSteps(), Waterfall())),
        Enterprise("Range bar", Chart("Daily temperature range", TemperatureRanges(), Range(AgChartSeriesTypes.RangeBar))),
        Enterprise("Range area", Chart("Forecast band", TemperatureRanges(), Range(AgChartSeriesTypes.RangeArea))),
        Enterprise("Nightingale", Chart("Seasonal rainfall", Seasons(), Polar(AgChartSeriesTypes.Nightingale))),
        Enterprise("Radar line", Chart("Team skills", Skills(), Polar(AgChartSeriesTypes.RadarLine))),
        Enterprise("Radar area", Chart("Product scores", Skills(), Polar(AgChartSeriesTypes.RadarArea))),
        Enterprise("Radial bar", Chart("Department headcount", Departments(), Polar(AgChartSeriesTypes.RadialBar))),
        Enterprise("Radial column", Chart("Regional sales", Departments(), Polar(AgChartSeriesTypes.RadialColumn))),
        Enterprise("Treemap", Chart("Catalogue size", Catalogue(), Hierarchy(AgChartSeriesTypes.Treemap))),
        Enterprise("Sunburst", Chart("Catalogue breakdown", Catalogue(), Hierarchy(AgChartSeriesTypes.Sunburst))),
        Enterprise("Sankey", Chart("Visitor flow", Flows(), Flow(AgChartSeriesTypes.Sankey))),
        Enterprise("Chord", Chart("Team collaboration", Flows(), Flow(AgChartSeriesTypes.Chord))),
        Enterprise("Funnel", Chart("Hiring funnel", Stages(), Stage(AgChartSeriesTypes.Funnel))),
        Enterprise("Cone funnel", Chart("Checkout drop-off", Stages(), Stage(AgChartSeriesTypes.ConeFunnel))),
        Enterprise("Pyramid", Chart("Population by age", Stages(), Stage(AgChartSeriesTypes.Pyramid))),
    ];

    private static ChartDemo Community(string name, AgChartOptions options) =>
        new(name, AgChartsEdition.Community, options);

    private static ChartDemo Enterprise(string name, AgChartOptions options) =>
        new(name, AgChartsEdition.Enterprise, options);

    private static AgChartOptions Chart(string title, object data, AgSeriesOptions series, bool legend = false) => new()
    {
        Title = new AgCaptionOptions { Text = title },
        Data = data,
        Series = [series],
        Legend = new AgLegendOptions { Enabled = legend },
    };

    private static AgSeriesOptions Area()
    {
        var series = AgSeriesOptions.Area("month", "visits", "Visits");
        series.FillOpacity = 0.45;
        return series;
    }

    private static AgSeriesOptions BoxPlot() => new()
    {
        Type = AgChartSeriesTypes.BoxPlot,
        XKey = "week",
        Additional = Keys(new
        {
            MinKey = "min",
            Q1Key = "q1",
            MedianKey = "median",
            Q3Key = "q3",
            MaxKey = "max",
        }),
    };

    private static AgSeriesOptions Ohlc(string type) => new()
    {
        Type = type,
        XKey = "date",
        Additional = Keys(new
        {
            OpenKey = "open",
            HighKey = "high",
            LowKey = "low",
            CloseKey = "close",
        }),
    };

    private static AgSeriesOptions Heatmap() => new()
    {
        Type = AgChartSeriesTypes.Heatmap,
        XKey = "day",
        YKey = "hour",
        Additional = Keys(new { ColorKey = "orders" }),
    };

    private static AgSeriesOptions Waterfall()
    {
        var series = new AgSeriesOptions
        {
            Type = AgChartSeriesTypes.Waterfall,
            XKey = "step",
            YKey = "amount",
        };
        series.Additional = new Dictionary<string, object?>
        {
            ["totals"] = new object[]
            {
                new { TotalType = "total", Index = 3, AxisLabel = "Result" },
            },
        };
        return series;
    }

    private static AgSeriesOptions Range(string type) => new()
    {
        Type = type,
        XKey = "day",
        Additional = Keys(new { YLowKey = "low", YHighKey = "high" }),
    };

    private static AgSeriesOptions Polar(string type) => new()
    {
        Type = type,
        Additional = Keys(new { AngleKey = "label", RadiusKey = "value" }),
    };

    private static AgSeriesOptions Hierarchy(string type) => new()
    {
        Type = type,
        Additional = Keys(new { LabelKey = "name", SizeKey = "size" }),
    };

    private static AgSeriesOptions Flow(string type) => new()
    {
        Type = type,
        Additional = Keys(new { FromKey = "from", ToKey = "to", SizeKey = "size" }),
    };

    private static AgSeriesOptions Stage(string type) => new()
    {
        Type = type,
        Additional = Keys(new { StageKey = "stage", ValueKey = "value" }),
    };

    private static Dictionary<string, object?> Keys(object values)
    {
        var keys = new Dictionary<string, object?>();
        foreach (var property in values.GetType().GetProperties())
        {
            var name = char.ToLowerInvariant(property.Name[0]) + property.Name[1..];
            keys[name] = property.GetValue(values);
        }

        return keys;
    }

    private static object[] Quarters() =>
    [
        new { Quarter = "Q1", Revenue = 120 },
        new { Quarter = "Q2", Revenue = 164 },
        new { Quarter = "Q3", Revenue = 149 },
        new { Quarter = "Q4", Revenue = 188 },
    ];

    private static object[] Months() =>
    [
        new { Month = "Jan", Visits = 3200 },
        new { Month = "Feb", Visits = 4100 },
        new { Month = "Mar", Visits = 3900 },
        new { Month = "Apr", Visits = 4700 },
        new { Month = "May", Visits = 5300 },
        new { Month = "Jun", Visits = 6100 },
    ];

    private static object[] Scores() =>
    [
        new { Hours = 2, Score = 48 },
        new { Hours = 4, Score = 61 },
        new { Hours = 5, Score = 58 },
        new { Hours = 7, Score = 74 },
        new { Hours = 8, Score = 81 },
        new { Hours = 11, Score = 92 },
    ];

    private static object[] Campaigns() =>
    [
        new { Spend = 12, Conversions = 40, Reach = 18 },
        new { Spend = 20, Conversions = 55, Reach = 32 },
        new { Spend = 28, Conversions = 49, Reach = 46 },
        new { Spend = 35, Conversions = 78, Reach = 27 },
        new { Spend = 48, Conversions = 96, Reach = 61 },
    ];

    private static object[] ResponseTimes() =>
        new[] { 42, 45, 47, 48, 51, 53, 54, 55, 58, 61, 63, 66, 70, 74, 81, 88, 96, 110 }
            .Select(value => new { Milliseconds = value })
            .Cast<object>()
            .ToArray();

    private static object[] Browsers() =>
    [
        new { Browser = "Chrome", Share = 58 },
        new { Browser = "Safari", Share = 21 },
        new { Browser = "Edge", Share = 12 },
        new { Browser = "Firefox", Share = 9 },
    ];

    private static object[] Tickets() =>
    [
        new { Channel = "Email", Count = 46 },
        new { Channel = "Chat", Count = 32 },
        new { Channel = "Phone", Count = 18 },
        new { Channel = "Portal", Count = 12 },
    ];

    private static object[] Deliveries() =>
    [
        new { Week = "W1", Min = 1, Q1 = 2, Median = 3, Q3 = 4, Max = 6 },
        new { Week = "W2", Min = 2, Q1 = 3, Median = 4, Q3 = 5, Max = 8 },
        new { Week = "W3", Min = 1, Q1 = 2, Median = 2, Q3 = 4, Max = 5 },
        new { Week = "W4", Min = 2, Q1 = 4, Median = 5, Q3 = 6, Max = 7 },
    ];

    private static object[] Prices() =>
    [
        new { Date = "2026-03-02", Open = 101, High = 108, Low = 99, Close = 106 },
        new { Date = "2026-03-03", Open = 106, High = 112, Low = 104, Close = 109 },
        new { Date = "2026-03-04", Open = 109, High = 111, Low = 102, Close = 103 },
        new { Date = "2026-03-05", Open = 103, High = 115, Low = 101, Close = 114 },
        new { Date = "2026-03-06", Open = 114, High = 118, Low = 110, Close = 112 },
    ];

    private static object[] Orders() =>
    [
        Cell("Mon", "09:00", 12), Cell("Mon", "12:00", 28), Cell("Mon", "15:00", 19),
        Cell("Tue", "09:00", 16), Cell("Tue", "12:00", 31), Cell("Tue", "15:00", 22),
        Cell("Wed", "09:00", 14), Cell("Wed", "12:00", 26), Cell("Wed", "15:00", 24),
        Cell("Thu", "09:00", 18), Cell("Thu", "12:00", 34), Cell("Thu", "15:00", 21),
    ];

    private static object Cell(string day, string hour, int orders) => new { Day = day, Hour = hour, Orders = orders };

    private static object[] ProfitSteps() =>
    [
        new { Step = "Sales", Amount = 180 },
        new { Step = "Services", Amount = 70 },
        new { Step = "Costs", Amount = -90 },
        new { Step = "Tax", Amount = -30 },
    ];

    private static object[] TemperatureRanges() =>
    [
        new { Day = "Mon", Low = 8, High = 15 },
        new { Day = "Tue", Low = 9, High = 17 },
        new { Day = "Wed", Low = 7, High = 14 },
        new { Day = "Thu", Low = 10, High = 18 },
        new { Day = "Fri", Low = 11, High = 16 },
    ];

    private static object[] Seasons() =>
    [
        new { Label = "Winter", Value = 82 },
        new { Label = "Spring", Value = 54 },
        new { Label = "Summer", Value = 31 },
        new { Label = "Autumn", Value = 67 },
    ];

    private static object[] Skills() =>
    [
        new { Label = "Design", Value = 72 },
        new { Label = "Build", Value = 84 },
        new { Label = "Test", Value = 63 },
        new { Label = "Support", Value = 77 },
        new { Label = "Sales", Value = 58 },
    ];

    private static object[] Departments() =>
    [
        new { Label = "North", Value = 42 },
        new { Label = "South", Value = 36 },
        new { Label = "East", Value = 51 },
        new { Label = "West", Value = 29 },
    ];

    private static object[] Catalogue() =>
    [
        new
        {
            Name = "Kitchen",
            Children = new object[]
            {
                new { Name = "Cookware", Size = 36 },
                new { Name = "Utensils", Size = 18 },
            },
        },
        new
        {
            Name = "Office",
            Children = new object[]
            {
                new { Name = "Paper", Size = 22 },
                new { Name = "Devices", Size = 44 },
            },
        },
    ];

    private static object[] Flows() =>
    [
        new { From = "Home", To = "Search", Size = 40 },
        new { From = "Home", To = "Offers", Size = 25 },
        new { From = "Search", To = "Product", Size = 32 },
        new { From = "Offers", To = "Product", Size = 14 },
        new { From = "Product", To = "Checkout", Size = 28 },
    ];

    private static object[] Stages() =>
    [
        new { Stage = "Visited", Value = 1000 },
        new { Stage = "Signed up", Value = 640 },
        new { Stage = "Qualified", Value = 310 },
        new { Stage = "Completed", Value = 140 },
    ];
}
