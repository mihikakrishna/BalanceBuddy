using System.Linq;
using BalanceBuddyDesktop.Models;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.VisualElements;
using SkiaSharp;

namespace BalanceBuddyDesktop.ViewModels.Charts
{
    public class ExpenseByCategoryChartViewModel
    {
        public ISeries[] Series { get; set; }

        public LabelVisual Title { get; set; } =
            new LabelVisual
            {
                Text = "Total Expenses by Category",
                TextSize = 25,
                Padding = new LiveChartsCore.Drawing.Padding(15),
                Paint = new SolidColorPaint(SKColors.DarkSlateGray)
            };

        public ExpenseByCategoryChartViewModel()
        {
            var expenses = GlobalData.Instance.Expenses;
            var totalByCategory = expenses
                .GroupBy(e => e.Category.Name)
                .Select(group => new
                {
                    CategoryName = group.Key,
                    TotalAmount = group.Sum(e => e.Amount)
                });

            Series = totalByCategory.Select(category => new PieSeries<decimal>
            {
                Values = new decimal[] { category.TotalAmount },
                Name = category.CategoryName
            }).ToArray();
        }
    }
}