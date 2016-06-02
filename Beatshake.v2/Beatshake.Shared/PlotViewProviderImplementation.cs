using Beatshake.DependencyServices;
using OxyPlot;

public class PlotViewProviderImplementation : IPlotViewProdiver
{
    public IPlotView GetPlotView()
    {
        return new OxyPlot.Windows.PlotView();
    }
}