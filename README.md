# RazorAgCharts

`RazorAgCharts` adds [AG Charts](https://www.ag-grid.com/charts/) to ASP.NET Core applications.

It supports:

- Razor Pages and MVC views through the `<ag-chart>` tag helper.
- Interactive Razor/Blazor components through the `<AgChart>` component.
- Typed chart, series, axis, legend, tooltip, and theme options.
- JavaScript formatters and renderers.
- C# or JavaScript click handlers.
- Full updates, delta updates, data transactions, image export, and chart state.
- AG Charts Community and Enterprise.

The wrapper currently defaults to AG Charts `14.2.0` and loads it from jsDelivr. You can configure another version or provide a self-hosted script URL.

## 1. Add the library

Add a project reference while developing both projects in the same solution:

```xml
<ItemGroup>
  <ProjectReference Include="..\RazorAgCharts\RazorAgCharts.csproj" />
</ItemGroup>
```

If the library has been packed and published, add its package instead:

```xml
<PackageReference Include="RazorAgCharts" Version="YOUR_VERSION" />
```

## 2. Register default settings

Registration is optional for Community charts, but recommended because it gives one place to configure the version, edition, CDN, or Enterprise licence.

In `Program.cs`:

```csharp
using RazorAgCharts;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddAgCharts();
```

Make sure static files and Razor Pages are mapped:

```csharp
var app = builder.Build();

app.UseStaticFiles();
app.MapRazorPages();

app.Run();
```

The wrapper's browser files are served from `/_content/RazorAgCharts/`.

## 3. Create chart options

Create a data type. Public C# property names are serialized to camel case, so `Month` becomes `month` and `IceCreamSales` becomes `iceCreamSales`.

```csharp
public sealed record MonthlySale(
    string Month,
    double AverageTemperature,
    double IceCreamSales);
```

Create the options in your Page Model, controller, or component:

```csharp
using RazorAgCharts;

var data = new[]
{
    new MonthlySale("Jan", 2.3, 162000),
    new MonthlySale("Mar", 6.3, 302000),
    new MonthlySale("May", 16.2, 800000),
    new MonthlySale("Jul", 22.8, 1254000),
};

var options = new AgChartOptions
{
    Title = new AgCaptionOptions
    {
        Text = "Ice Cream Sales",
    },
    Data = data,
    Series =
    [
        AgSeriesOptions.Bar(
            xKey: "month",
            yKey: "iceCreamSales",
            yName: "Ice Cream Sales"),
        AgSeriesOptions.Line(
            xKey: "month",
            yKey: "averageTemperature",
            yName: "Average Temperature"),
    ],
    Legend = new AgLegendOptions
    {
        Position = AgLegendPositions.Bottom,
    },
};
```

The keys supplied to a series must match the serialized data names, not necessarily the original C# capitalization.

## Razor Pages

### 1. Enable the tag helper

Add the following to `Pages/_ViewImports.cshtml`:

```cshtml
@using RazorAgCharts
@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers
@addTagHelper *, RazorAgCharts
```

### 2. Expose options from the Page Model

`Pages/Index.cshtml.cs`:

```csharp
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorAgCharts;

public class IndexModel : PageModel
{
    public AgChartOptions ChartOptions { get; private set; } = new();

    public void OnGet()
    {
        ChartOptions = new AgChartOptions
        {
            Title = new AgCaptionOptions { Text = "Monthly Sales" },
            Data = new[]
            {
                new { Month = "Jan", Sales = 42000 },
                new { Month = "Feb", Sales = 51000 },
                new { Month = "Mar", Sales = 63000 },
            },
            Series =
            [
                AgSeriesOptions.Bar("month", "sales", "Sales"),
            ],
        };
    }
}
```

### 3. Render the chart

`Pages/Index.cshtml`:

```cshtml
@page
@model IndexModel

<ag-chart options="Model.ChartOptions"
          width="100%"
          height="420px"
          aria-label="Monthly sales chart">
</ag-chart>
```

The tag helper automatically adds its stylesheet and JavaScript loader once per rendered page. You do not need to add AG Charts `<script>` tags to the layout.

### Razor Pages events

Tag helper events call functions attached to `window`. Pass the function's name, without parentheses:

```cshtml
<ag-chart options="Model.ChartOptions"
          on-series-node-click="salesPointClicked"
          on-series-node-double-click="salesPointDoubleClicked"
          on-click="chartBackgroundClicked">
</ag-chart>

<p id="chart-status" role="status"></p>

<script>
    window.salesPointClicked = (event) => {
        const month = event.datum?.month;
        const sales = event.datum?.sales;
        document.getElementById("chart-status").textContent =
            `${month}: ${Number(sales).toLocaleString()}`;
    };

    window.salesPointDoubleClicked = (event) => {
        console.log("Double-clicked", event.datum);
    };

    window.chartBackgroundClicked = (event) => {
        console.log("Chart coordinates", event.coordinates);
    };
</script>
```

Available attributes are:

- `on-series-node-click`
- `on-series-node-double-click`
- `on-click`
- `on-double-click`

## Interactive Razor/Blazor components

The component requires an interactive render mode because it creates the chart through JavaScript interop.

### 1. Register Razor components

```csharp
using RazorAgCharts;

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddAgCharts();

// ...

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();
```

Add the namespace to `Components/_Imports.razor`:

```razor
@using RazorAgCharts
```

### 2. Render a component

```razor
@page "/sales"
@rendermode InteractiveServer

<AgChart Options="_options"
         Width="100%"
         Height="420px"
         AriaLabel="Monthly sales"
         OnSeriesNodeClick="HandlePointClick" />

<p role="status">@_status</p>

@code {
    private string _status = "Select a bar.";

    private readonly AgChartOptions _options = new()
    {
        Title = new AgCaptionOptions { Text = "Monthly Sales" },
        Data = new[]
        {
            new { Month = "Jan", Sales = 42000 },
            new { Month = "Feb", Sales = 51000 },
            new { Month = "Mar", Sales = 63000 },
        },
        Series =
        [
            AgSeriesOptions.Bar("month", "sales", "Sales"),
        ],
    };

    private Task HandlePointClick(AgChartEventArgs args)
    {
        var month = args.Datum.GetProperty("month").GetString();
        var sales = args.Datum.GetProperty("sales").GetDouble();
        _status = $"{month}: {sales:N0}";
        return Task.CompletedTask;
    }
}
```

Component events are:

- `OnSeriesNodeClick`
- `OnSeriesNodeDoubleClick`
- `OnClick`
- `OnDoubleClick`

Use `args.GetDatum<T>()` to deserialize a clicked datum into a C# type:

```csharp
private Task HandlePointClick(AgChartEventArgs args)
{
    var sale = args.GetDatum<MonthlySale>();
    // Use sale...
    return Task.CompletedTask;
}
```

## Updating a Blazor chart

### Replace the complete options

Assign a new options instance and allow the component to render again. The wrapper calls AG Charts `update` when the serialized options change:

```csharp
private void ReplaceData()
{
    _options = new AgChartOptions
    {
        Data = updatedData,
        Series = [AgSeriesOptions.Bar("month", "sales")],
    };
}
```

AG Charts recommends immutable data objects so that changes can be detected reliably.

### Apply a partial update

Capture the component with `@ref`:

```razor
<AgChart @ref="_chart" Options="_options" />

@code {
    private AgChart? _chart;
}
```

Then call `UpdateDeltaAsync` after the chart has rendered:

```csharp
await _chart!.UpdateDeltaAsync(new AgChartOptions
{
    Title = new AgCaptionOptions { Text = "Updated title" },
    Theme = AgChartThemes.Vivid,
});
```

When a delta changes `series` or `axes`, provide the complete series list or complete axis definitions required by AG Charts.

### Apply a data transaction

For frequent data changes:

```csharp
await _chart!.ApplyTransactionAsync(new AgDataTransaction
{
    Add =
    [
        new MonthlySale("Apr", 12.5, 71000),
    ],
});
```

Set `DataIdKey` in `AgChartOptions` when AG Charts should match updated or removed rows by a unique field:

```csharp
var options = new AgChartOptions
{
    DataIdKey = "id",
    Data = rows,
    Series = [AgSeriesOptions.Line("time", "value")],
};
```

## Export and state

These methods are available from a Blazor `AgChart` component reference after its first interactive render:

```csharp
await _chart!.DownloadAsync(new AgChartImageOptions
{
    FileName = "monthly-sales",
    Width = 1200,
    Height = 700,
});

string imageDataUrl = await _chart.GetImageDataUrlAsync();

string state = await _chart.GetStateAsync();
await _chart.SetStateAsync(state);

await _chart.WaitForUpdateAsync();
```

## Axes and JavaScript formatters

Define axes in a dictionary. Cartesian charts normally use `x` and `y`; named axes can be linked through `XKeyAxis` or `YKeyAxis`.

```csharp
var sales = AgSeriesOptions.Bar("month", "sales", "Sales");
sales.YKeyAxis = "salesAxis";

var options = new AgChartOptions
{
    Data = data,
    Series = [sales],
    Axes = new Dictionary<string, AgAxisOptions>
    {
        ["x"] = new()
        {
            Type = AgAxisTypes.Category,
            Position = AgAxisPositions.Bottom,
        },
        ["salesAxis"] = new()
        {
            Type = AgAxisTypes.Number,
            Position = AgAxisPositions.Left,
            Title = new AgAxisTitleOptions { Text = "Sales (USD)" },
            Label = new AgAxisLabelOptions
            {
                Formatter = new JsCallback(
                    "(params) => '$' + Number(params.value).toLocaleString()"),
            },
        },
    },
};
```

`JsCallback` is also supported for tooltip renderers and AG Charts listeners. Its content is compiled as JavaScript in the browser. Only use function expressions from trusted application code; never construct one from user input.

## Options not represented by a typed property

AG Charts has a large and frequently changing API. Use `Additional` for any valid option that is not exposed as a typed C# property. Keys must use the exact JavaScript names from the [AG Charts options reference](https://www.ag-grid.com/charts/javascript/).

```csharp
var series = AgSeriesOptions.Bar("month", "sales");
series.Additional = new Dictionary<string, object?>
{
    ["highlightStyle"] = new
    {
        Item = new
        {
            Fill = "gold",
        },
    },
};
```

`Additional` values are merged into the generated options object. If an additional key matches a typed property, the additional value wins.

You can also bypass the typed model with `OptionsJson`:

```razor
<AgChart OptionsJson="@rawOptionsJson" />
```

```cshtml
<ag-chart options-json="@Model.RawOptionsJson"></ag-chart>
```

Raw JSON must already use AG Charts' JavaScript property names. Prefer typed options when possible.

## Themes

Set a built-in theme:

```csharp
options.Theme = AgChartThemes.MaterialDark;
```

Available constants include:

- `Default` / `DefaultDark`
- `Sheets` / `SheetsDark`
- `Polychroma` / `PolychromaDark`
- `Vivid` / `VividDark`
- `Material` / `MaterialDark`

Use `AgChartTheme` for a custom theme:

```csharp
options.Theme = new AgChartTheme
{
    BaseTheme = AgChartThemes.Default,
    Palette = new
    {
        Fills = new[] { "#2563eb", "#16a34a", "#f97316" },
        Strokes = new[] { "#1e40af", "#166534", "#c2410c" },
    },
};
```

## Enterprise charts

Configure Enterprise globally:

```csharp
builder.Services.AddAgCharts(settings =>
{
    settings.Edition = AgChartsEdition.Enterprise;
    settings.LicenseKey = builder.Configuration["AgCharts:LicenseKey"];
});
```

The licence key is necessarily sent to the browser because AG Charts validates it client-side. Do not treat it as a server-side secret.

Select an Enterprise preset:

```razor
<AgChart Preset="AgChartPreset.Financial"
         Options="_financialOptions"
         Height="600px" />
```

```cshtml
<ag-chart preset="AgChartPreset.Financial"
          options="Model.FinancialOptions"
          height="600px">
</ag-chart>
```

`Financial`, `Gauge`, and `Quadrant` presets automatically select the Enterprise package.

## Version and script hosting

Change the AG Charts version globally:

```csharp
builder.Services.AddAgCharts(settings =>
{
    settings.Version = "14.2.0";
});
```

To avoid a public CDN, copy the AG Charts UMD bundle into your application's `wwwroot` and configure its URL:

```csharp
builder.Services.AddAgCharts(settings =>
{
    settings.CommunityModuleUrl = "/js/vendor/ag-charts-community.min.js";
});
```

URLs ending in `.js` are loaded as normal script tags. URLs ending in `.mjs`, containing `/esm/`, or containing `+esm` are imported as ES modules.

If your Content Security Policy blocks jsDelivr, either allow the configured CDN in `script-src` or self-host the bundle.

## Troubleshooting

### The chart area is blank

- Give the chart a non-zero height. The default is `400px`.
- Confirm that `/_content/RazorAgCharts/agChartsInterop.js` or `agCharts.auto.js` returns HTTP 200.
- Check the browser console for a blocked CDN request or invalid AG Charts option.
- Confirm that each series key matches the serialized data property name.

### The Blazor chart never appears

- Use an interactive render mode such as `@rendermode InteractiveServer`.
- Ensure interactive components are registered and mapped in `Program.cs`.
- Do not call methods on `@ref` before the first interactive render completes.

### A Razor Pages event does not fire

- Attach the callback to `window`.
- Pass only its name to the tag helper, such as `on-series-node-click="pointClicked"`.
- Do not pass `pointClicked()` or an inline JavaScript expression.

### A JavaScript formatter fails

`JsCallback` must contain a complete function expression:

```csharp
new JsCallback("(params) => params.value + ' °C'")
```

It cannot contain only the function body or a C# delegate.

## Example application

The `samples/RazorAgCharts.Sample` project contains:

- A traditional Razor Page using the tag helper.
- An interactive server component using C# events.
- Bar, line, and pie series.
- A gallery at `/gallery` with one example of every supported series type.
- Multiple axes and JavaScript label formatters.
- Runtime data and theme updates.
- PNG download.

Run it from the repository root:

```powershell
dotnet run --project samples/RazorAgCharts.Sample/RazorAgCharts.Sample.csproj
```
