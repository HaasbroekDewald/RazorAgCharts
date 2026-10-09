using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;

namespace RazorAgCharts;

/// <summary>
/// Renders an AG Chart. Use this component from a Blazor page or from a Razor Page that hosts interactive components.
/// For a Razor Page without a Blazor circuit, use the <c>ag-chart</c> tag helper instead.
/// </summary>
public partial class AgChart : IAsyncDisposable
{
    private readonly string _chartId = $"agchart-{Guid.NewGuid():N}";
    private readonly DotNetObjectReference<AgChart> _dotNetRef;
    private ElementReference _container;
    private IJSObjectReference? _module;
    private string? _appliedJson;
    private string? _appliedSettings;
    private string? _error;
    private bool _rendered;
    private bool _disposed;

    public AgChart()
    {
        _dotNetRef = DotNetObjectReference.Create(this);
    }

    [Inject] private IJSRuntime Js { get; set; } = default!;

    [Inject] private NavigationManager Navigation { get; set; } = default!;

    [Inject] private IOptions<AgChartsSettings> Settings { get; set; } = default!;

    /// <summary>Full chart options. A change serializes the options and calls <c>chart.update</c>.</summary>
    [Parameter] public AgChartOptions? Options { get; set; }

    /// <summary>
    /// Raw AG Charts options JSON used when <see cref="Options"/> is null.
    /// Property names must already be the JavaScript names.
    /// </summary>
    [Parameter] public string? OptionsJson { get; set; }

    [Parameter] public AgChartPreset Preset { get; set; } = AgChartPreset.Chart;

    [Parameter] public AgChartsEdition? Edition { get; set; }

    [Parameter] public string? Version { get; set; }

    [Parameter] public string? LicenseKey { get; set; }

    [Parameter] public string? CommunityModuleUrl { get; set; }

    [Parameter] public string? EnterpriseModuleUrl { get; set; }

    [Parameter] public string Width { get; set; } = "100%";

    [Parameter] public string Height { get; set; } = "400px";

    [Parameter] public string? CssClass { get; set; }

    [Parameter] public string? AriaLabel { get; set; }

    [Parameter] public EventCallback<AgChartEventArgs> OnSeriesNodeClick { get; set; }

    [Parameter] public EventCallback<AgChartEventArgs> OnSeriesNodeDoubleClick { get; set; }

    [Parameter] public EventCallback<AgChartEventArgs> OnClick { get; set; }

    [Parameter] public EventCallback<AgChartEventArgs> OnDoubleClick { get; set; }

    private string HostStyle => $"width:{Width};height:{Height};";

    private string ComputedAriaLabel => AriaLabel ?? Options?.Title?.Text ?? "Chart";

    /// <summary>Applies a partial options object with <c>chart.updateDelta</c>.</summary>
    public async Task UpdateDeltaAsync(AgChartOptions delta)
    {
        ArgumentNullException.ThrowIfNull(delta);
        await UpdateDeltaAsync(AgChartsJson.Serialize(delta));
    }

    /// <summary>Applies partial options JSON with <c>chart.updateDelta</c>.</summary>
    public async Task UpdateDeltaAsync(string optionsJson)
    {
        var module = await RequireChartAsync();
        await module.InvokeVoidAsync("updateDelta", _chartId, optionsJson);
    }

    /// <summary>Incrementally changes chart data with <c>chart.applyTransaction</c>.</summary>
    public async Task ApplyTransactionAsync(AgDataTransaction transaction)
    {
        ArgumentNullException.ThrowIfNull(transaction);
        var module = await RequireChartAsync();
        await module.InvokeVoidAsync("applyTransaction", _chartId, AgChartsJson.Serialize(transaction));
    }

    /// <summary>Starts a PNG download of the chart.</summary>
    public async Task DownloadAsync(AgChartImageOptions? options = null)
    {
        var module = await RequireChartAsync();
        await module.InvokeVoidAsync("downloadChart", _chartId, options);
    }

    /// <summary>Returns a base64 PNG data URL for the chart.</summary>
    public async Task<string> GetImageDataUrlAsync(AgChartImageOptions? options = null)
    {
        var module = await RequireChartAsync();
        return await module.InvokeAsync<string>("getImageDataUrl", _chartId, options);
    }

    /// <summary>Returns the current chart state JSON.</summary>
    public async Task<string> GetStateAsync()
    {
        var module = await RequireChartAsync();
        return await module.InvokeAsync<string>("getChartState", _chartId);
    }

    /// <summary>Restores chart state previously returned by <see cref="GetStateAsync"/>.</summary>
    public async Task SetStateAsync(string stateJson)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(stateJson);
        var module = await RequireChartAsync();
        await module.InvokeVoidAsync("setChartState", _chartId, stateJson);
    }

    /// <summary>Waits until the current chart update has rendered.</summary>
    public async Task WaitForUpdateAsync()
    {
        var module = await RequireChartAsync();
        await module.InvokeVoidAsync("waitForChartUpdate", _chartId);
    }

    [JSInvokable]
    public Task OnSeriesNodeClickFromJs(JsonElement payload) =>
        OnSeriesNodeClick.InvokeAsync(AgChartEventArgs.From(payload));

    [JSInvokable]
    public Task OnSeriesNodeDoubleClickFromJs(JsonElement payload) =>
        OnSeriesNodeDoubleClick.InvokeAsync(AgChartEventArgs.From(payload));

    [JSInvokable]
    public Task OnClickFromJs(JsonElement payload) =>
        OnClick.InvokeAsync(AgChartEventArgs.From(payload));

    [JSInvokable]
    public Task OnDoubleClickFromJs(JsonElement payload) =>
        OnDoubleClick.InvokeAsync(AgChartEventArgs.From(payload));

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        _rendered = true;
        if (_disposed)
        {
            return;
        }

        if (!HasOptions)
        {
            const string missing = "Set Options or OptionsJson on AgChart.";
            if (_error != missing)
            {
                _error = missing;
                await InvokeAsync(StateHasChanged);
            }

            return;
        }

        string? message = null;
        try
        {
            await SyncAsync();
        }
        catch (JSDisconnectedException)
        {
            return;
        }
        catch (Exception ex)
        {
            message = ex.Message;
        }

        if (_error != message)
        {
            _error = message;
            await InvokeAsync(StateHasChanged);
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        _dotNetRef.Dispose();

        if (_module is null)
        {
            return;
        }

        try
        {
            await _module.InvokeVoidAsync("destroyChart", _chartId);
            await _module.DisposeAsync();
        }
        catch (JSDisconnectedException)
        {
        }
    }

    private bool HasOptions => Options is not null || !string.IsNullOrWhiteSpace(OptionsJson);

    private async Task<IJSObjectReference> RequireChartAsync()
    {
        if (_disposed || !_rendered)
        {
            throw new InvalidOperationException("The chart has not been rendered yet.");
        }

        await SyncAsync();
        return _module ?? throw new InvalidOperationException("The AG Charts browser module is not loaded.");
    }

    private async Task SyncAsync()
    {
        if (!HasOptions)
        {
            throw new InvalidOperationException("Set AgChart.Options or AgChart.OptionsJson.");
        }

        var module = await EnsureModuleAsync();
        var json = Options is not null ? AgChartsJson.Serialize(Options) : OptionsJson!;
        var settings = CreateClientSettings();
        var settingsKey = AgChartsJson.Serialize(settings);
        if (json == _appliedJson && settingsKey == _appliedSettings)
        {
            return;
        }

        await module.InvokeVoidAsync("syncChart", _chartId, _container, json, settings, _dotNetRef);
        _appliedJson = json;
        _appliedSettings = settingsKey;
    }

    private async Task<IJSObjectReference> EnsureModuleAsync()
    {
        if (_module is not null)
        {
            return _module;
        }

        var url = Navigation.ToAbsoluteUri("_content/RazorAgCharts/agChartsInterop.js");
        _module = await Js.InvokeAsync<IJSObjectReference>("import", url.ToString());
        return _module;
    }

    private object CreateClientSettings()
    {
        var configured = Settings.Value ?? new AgChartsSettings();
        var version = string.IsNullOrWhiteSpace(Version) ? configured.Version : Version;
        var edition = Edition ?? configured.Edition;
        if (Preset != AgChartPreset.Chart)
        {
            edition = AgChartsEdition.Enterprise;
        }

        return new
        {
            Preset = ToPresetName(Preset),
            Edition = edition == AgChartsEdition.Enterprise ? "enterprise" : "community",
            Version = version,
            LicenseKey = LicenseKey ?? configured.LicenseKey,
            CommunityModuleUrl = CommunityModuleUrl
                ?? configured.CommunityModuleUrl
                ?? AgChartsDefaults.CommunityScript(version),
            EnterpriseModuleUrl = EnterpriseModuleUrl
                ?? configured.EnterpriseModuleUrl
                ?? AgChartsDefaults.EnterpriseScript(version),
            SeriesNodeClick = OnSeriesNodeClick.HasDelegate,
            SeriesNodeDoubleClick = OnSeriesNodeDoubleClick.HasDelegate,
            Click = OnClick.HasDelegate,
            DoubleClick = OnDoubleClick.HasDelegate,
        };
    }

    private static string ToPresetName(AgChartPreset preset) => preset switch
    {
        AgChartPreset.Financial => "financial",
        AgChartPreset.Gauge => "gauge",
        AgChartPreset.Quadrant => "quadrant",
        _ => "chart",
    };
}
