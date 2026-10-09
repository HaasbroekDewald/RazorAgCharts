namespace RazorAgCharts;

/// <summary>Defaults applied by <see cref="AgChart"/> and the <c>ag-chart</c> tag helper when a page does not override them.</summary>
public sealed class AgChartsSettings
{
    public string Version { get; set; } = AgChartsDefaults.Version;

    public AgChartsEdition Edition { get; set; } = AgChartsEdition.Community;

    /// <summary>Enterprise or trial licence. It is sent to the browser because AG Charts validates it there.</summary>
    public string? LicenseKey { get; set; }

    /// <summary>Overrides the community script. A <c>.js</c> URL is loaded as a script tag. A <c>.mjs</c> or <c>+esm</c> URL is imported as a module.</summary>
    public string? CommunityModuleUrl { get; set; }

    /// <summary>Overrides the enterprise script. Same URL rules as <see cref="CommunityModuleUrl"/>.</summary>
    public string? EnterpriseModuleUrl { get; set; }
}
