using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorAgCharts;

namespace RazorAgCharts.Sample.Pages;

public class IndexModel : PageModel
{
    public AgChartOptions Options { get; private set; } = SampleCharts.IceCream();

    public void OnGet()
    {
        Options = SampleCharts.IceCream();
    }
}
