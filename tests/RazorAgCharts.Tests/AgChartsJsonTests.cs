using System.Text.Json.Nodes;
using RazorAgCharts;

namespace RazorAgCharts.Tests;

public class AgChartsJsonTests
{
    [Fact]
    public void Serialize_UsesCamelCaseAndOmitsNulls()
    {
        var json = AgChartsJson.Serialize(new AgChartOptions
        {
            Title = new AgCaptionOptions { Text = "Ice Cream Sales" },
            Data = new[] { new MonthSale("Jan", 162000) },
            Series = [AgSeriesOptions.Bar("month", "iceCreamSales", "Ice Cream Sales")],
        });

        var node = JsonNode.Parse(json)!.AsObject();
        Assert.Equal("Ice Cream Sales", node["title"]!["text"]!.GetValue<string>());
        Assert.Equal("Jan", node["data"]![0]!["month"]!.GetValue<string>());
        Assert.Equal(162000, node["data"]![0]!["iceCreamSales"]!.GetValue<int>());
        Assert.Equal("bar", node["series"]![0]!["type"]!.GetValue<string>());
        Assert.Equal("month", node["series"]![0]!["xKey"]!.GetValue<string>());
        Assert.Equal("iceCreamSales", node["series"]![0]!["yKey"]!.GetValue<string>());
        Assert.False(node.ContainsKey("subtitle"));
        Assert.False(node.ContainsKey("container"));
        Assert.DoesNotContain("subtitle", json, StringComparison.Ordinal);
    }

    [Fact]
    public void Serialize_MergesAdditionalAndRevivesCallbacks()
    {
        var json = AgChartsJson.Serialize(new AgChartOptions
        {
            Title = new AgCaptionOptions { Text = "Sales" },
            Axes = new Dictionary<string, AgAxisOptions>
            {
                ["y"] = new()
                {
                    Type = AgAxisTypes.Number,
                    Label = new AgAxisLabelOptions
                    {
                        Formatter = new JsCallback("(params) => params.value + ' °C'"),
                    },
                },
            },
            Additional = new Dictionary<string, object?>
            {
                ["footnote"] = new AgCaptionOptions { Text = "FY22" },
                ["title"] = new AgCaptionOptions { Text = "Override" },
            },
        });

        var node = JsonNode.Parse(json)!.AsObject();
        Assert.False(node.ContainsKey("additional"));
        Assert.Equal("Override", node["title"]!["text"]!.GetValue<string>());
        Assert.Equal("FY22", node["footnote"]!["text"]!.GetValue<string>());
        Assert.Equal("(params) => params.value + ' °C'", node["axes"]!["y"]!["label"]!["formatter"]!["$js"]!.GetValue<string>());
    }

    [Fact]
    public void Serialize_EscapesHtmlInText()
    {
        var json = AgChartsJson.Serialize(new AgChartOptions
        {
            Title = new AgCaptionOptions { Text = "<script>alert(1)</script>" },
        });

        Assert.DoesNotContain("<script>", json, StringComparison.Ordinal);
    }

    private sealed record MonthSale(string Month, double IceCreamSales);
}
