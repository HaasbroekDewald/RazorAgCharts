using Microsoft.Extensions.DependencyInjection;

namespace RazorAgCharts;

public static class AgChartsServiceCollectionExtensions
{
    /// <summary>
    /// Stores default AG Charts version, edition, and licence key for <see cref="AgChart"/> and the <c>ag-chart</c> tag helper.
    /// </summary>
    public static IServiceCollection AddAgCharts(this IServiceCollection services, Action<AgChartsSettings>? configure = null)
    {
        if (configure is not null)
        {
            services.Configure(configure);
        }

        return services;
    }
}
