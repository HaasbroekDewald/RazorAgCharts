using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace RazorAgCharts.TagHelpers;

/// <summary>
/// Renders an AG Chart on a Razor Page or MVC view without a Blazor circuit.
/// Add <c>@addTagHelper *, RazorAgCharts</c> to <c>_ViewImports.cshtml</c>.
/// Click handlers are names of functions on <c>window</c>, called with the sanitized event payload.
/// </summary>
[HtmlTargetElement("ag-chart")]
public sealed class AgChartTagHelper : TagHelper
{
    private const string ScriptItemKey = "RazorAgCharts.Script";

    public AgChartOptions? Options { get; set; }

    public string? OptionsJson { get; set; }

    public string Height { get; set; } = "400px";

    public string Width { get; set; } = "100%";

    public string? CssClass { get; set; }

    public AgChartPreset Preset { get; set; } = AgChartPreset.Chart;

    public AgChartsEdition? Edition { get; set; }

    public string? Version { get; set; }

    public string? LicenseKey { get; set; }

    public string? CommunityModuleUrl { get; set; }

    public string? EnterpriseModuleUrl { get; set; }

    public string? AriaLabel { get; set; }

    /// <summary>Name of a <c>window</c> function called when a series node is clicked.</summary>
    public string? OnSeriesNodeClick { get; set; }

    public string? OnSeriesNodeDoubleClick { get; set; }

    public string? OnClick { get; set; }

    public string? OnDoubleClick { get; set; }

    [HtmlAttributeNotBound]
    [ViewContext]
    public ViewContext? ViewContext { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        var configured = ViewContext?.HttpContext?.RequestServices?.GetService<IOptions<AgChartsSettings>>()?.Value
            ?? new AgChartsSettings();
        var version = string.IsNullOrWhiteSpace(Version) ? configured.Version : Version;
        var edition = Edition ?? configured.Edition;
        if (Preset != AgChartPreset.Chart)
        {
            edition = AgChartsEdition.Enterprise;
        }

        var id = $"agchart-{Guid.NewGuid():N}";
        var json = Options is not null
            ? AgChartsJson.Serialize(Options)
            : string.IsNullOrWhiteSpace(OptionsJson) ? "{}" : OptionsJson;
        var pathBase = ViewContext?.HttpContext.Request.PathBase.Value ?? "";
        var extraClass = output.Attributes.TryGetAttribute("class", out var classAttribute)
            ? classAttribute.Value?.ToString()
            : null;

        RemoveBoundAttributes(output);

        output.TagName = "div";
        output.TagMode = TagMode.StartTagAndEndTag;
        output.Attributes.SetAttribute("id", id);
        output.Attributes.SetAttribute("class", JoinClasses("ag-chart-host", CssClass, extraClass));
        output.Attributes.SetAttribute("style", $"width:{Width};height:{Height};");
        output.Attributes.SetAttribute("data-ag-chart", "true");
        output.Attributes.SetAttribute("data-ag-chart-preset", ToPresetName(Preset));
        output.Attributes.SetAttribute("data-ag-chart-edition", edition == AgChartsEdition.Enterprise ? "enterprise" : "community");
        output.Attributes.SetAttribute("data-ag-chart-version", version);
        output.Attributes.SetAttribute("role", "img");
        output.Attributes.SetAttribute("aria-label", AriaLabel ?? Options?.Title?.Text ?? "Chart");
        SetOptional(output, "data-ag-chart-license", LicenseKey ?? configured.LicenseKey);
        SetOptional(output, "data-ag-chart-community-module", CommunityModuleUrl ?? configured.CommunityModuleUrl);
        SetOptional(output, "data-ag-chart-enterprise-module", EnterpriseModuleUrl ?? configured.EnterpriseModuleUrl);
        SetOptional(output, "data-ag-chart-series-node-click", OnSeriesNodeClick);
        SetOptional(output, "data-ag-chart-series-node-double-click", OnSeriesNodeDoubleClick);
        SetOptional(output, "data-ag-chart-click", OnClick);
        SetOptional(output, "data-ag-chart-double-click", OnDoubleClick);

        output.Content.SetHtmlContent(string.Empty);
        output.PostElement.AppendHtml(
            $"""<script type="application/json" data-ag-chart-options="{id}">{json}</script>""");

        if (!context.Items.ContainsKey(ScriptItemKey))
        {
            context.Items[ScriptItemKey] = true;
            output.PostElement.AppendHtml(
                $"""<link rel="stylesheet" href="{pathBase}/_content/RazorAgCharts/agCharts.css" /><script type="module" src="{pathBase}/_content/RazorAgCharts/agCharts.auto.js"></script>""");
        }
    }

    private static void RemoveBoundAttributes(TagHelperOutput output)
    {
        string[] names =
        [
            "options",
            "options-json",
            "height",
            "width",
            "css-class",
            "preset",
            "edition",
            "version",
            "license-key",
            "community-module-url",
            "enterprise-module-url",
            "aria-label",
            "on-series-node-click",
            "on-series-node-double-click",
            "on-click",
            "on-double-click",
            "class",
        ];

        foreach (var name in names)
        {
            output.Attributes.RemoveAll(name);
        }
    }

    private static void SetOptional(TagHelperOutput output, string name, string? value)
    {
        if (!string.IsNullOrWhiteSpace(value))
        {
            output.Attributes.SetAttribute(name, value);
        }
    }

    private static string JoinClasses(params string?[] classes) =>
        string.Join(' ', classes.Where(value => !string.IsNullOrWhiteSpace(value)));

    private static string ToPresetName(AgChartPreset preset) => preset switch
    {
        AgChartPreset.Financial => "financial",
        AgChartPreset.Gauge => "gauge",
        AgChartPreset.Quadrant => "quadrant",
        _ => "chart",
    };
}
