using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.VisualElements;
using SkiaSharp;
using BalanceBuddyDesktop.ViewModels.Charts;


namespace BalanceBuddyDesktop.ViewModels;

public class ViewExpensesPageViewModel : ViewModelBase
{
    public ExpenseByCategoryChartViewModel ExpenseByCategoryChartViewModel { get; } = new();
}
