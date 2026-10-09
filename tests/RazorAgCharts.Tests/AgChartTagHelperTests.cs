using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using RazorAgCharts;
using RazorAgCharts.TagHelpers;

namespace RazorAgCharts.Tests;

public class AgChartTagHelperTests
{
    [Fact]
    public void Process_WritesChartHostOptionsAndLoaderOnce()
    {
        var context = new TagHelperContext(
            new TagHelperAttributeList(),
            new Dictionary<object, object>(),
            "chart");
        var first = Render(context, new AgChartTagHelper
        {
            Options = new AgChartOptions
            {
                Title = new AgCaptionOptions { Text = "<script>alert(1)</script>" },
            },
            Height = "320px",
            OnSeriesNodeClick = "showSale",
            ViewContext = new ViewContext { HttpContext = new DefaultHttpContext() },
        });
        var second = Render(context, new AgChartTagHelper
        {
            OptionsJson = """{"series":[{"type":"line","xKey":"month","yKey":"avgTemp"}]}""",
            ViewContext = new ViewContext { HttpContext = new DefaultHttpContext() },
        });

        Assert.Equal("div", first.TagName);
        Assert.Equal("true", first.Attributes["data-ag-chart"].Value.ToString());
        Assert.Equal("320px", StyleValue(first, "height"));
        Assert.Equal("showSale", first.Attributes["data-ag-chart-series-node-click"].Value.ToString());
        Assert.Contains("agCharts.auto.js", first.PostElement.GetContent(), StringComparison.Ordinal);
        Assert.DoesNotContain("<script>alert", first.PostElement.GetContent(), StringComparison.Ordinal);
        Assert.Contains("data-ag-chart-options", first.PostElement.GetContent(), StringComparison.Ordinal);

        Assert.DoesNotContain("agCharts.auto.js", second.PostElement.GetContent(), StringComparison.Ordinal);
        Assert.Contains("avgTemp", second.PostElement.GetContent(), StringComparison.Ordinal);
    }

    private static TagHelperOutput Render(TagHelperContext context, AgChartTagHelper helper)
    {
        var output = new TagHelperOutput(
            "ag-chart",
            [],
            (_, _) => Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));
        helper.Process(context, output);
        return output;
    }

    private static string StyleValue(TagHelperOutput output, string property)
    {
        var style = output.Attributes["style"].Value?.ToString() ?? "";
        var part = style.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .First(value => value.StartsWith(property + ":", StringComparison.Ordinal));
        return part[(property.Length + 1)..];
    }
}
