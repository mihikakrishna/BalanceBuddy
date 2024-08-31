using System;
using System.Linq;
using BalanceBuddyDesktop.Models;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace BalanceBuddyDesktop.ViewModels.Charts
{
    public class StackedTransactionChartViewModel
    {
        public ISeries[] Series { get; set; }

        public Axis[] XAxes { get; set; } = new[]
        {
            new DateTimeAxis(TimeSpan.FromDays(30), date => date.ToString("MMMM yyyy"))
            {
                LabelsRotation = 15,
                Name = "Month",
                SeparatorsPaint = new SolidColorPaint { Color = SKColors.Gray },
                UnitWidth = TimeSpan.FromDays(30).Ticks 
            }
        };

        public Axis[] YAxes { get; set; } = new[]
        {
            new Axis
            {
                Labeler = value => value.ToString("C"),
                Name = "Amount"
            }
        };

        public StackedTransactionChartViewModel()
        {
            var monthlyExpenses = GlobalData.Instance.Expenses
                .GroupBy(e => new { e.Date.Year, e.Date.Month })
                .Select(g => new
                {
                    Date = new DateTime(g.Key.Year, g.Key.Month, 1),
                    Total = g.Sum(e => e.Amount)
                })
                .OrderBy(m => m.Date);

            var monthlyIncome = GlobalData.Instance.Incomes
                .GroupBy(i => new { i.Date.Year, i.Date.Month })
                .Select(g => new
                {
                    Date = new DateTime(g.Key.Year, g.Key.Month, 1),
                    Total = g.Sum(i => i.Amount)
                })
                .OrderBy(m => m.Date);


            if (!monthlyIncome.Any() && !monthlyExpenses.Any())
            {
                Series = new ISeries[]
                {
                    new ColumnSeries<DateTimePoint>
                    {
                        Values = new [] { new DateTimePoint(DateTime.Now, 0) },
                        Name = "No data to show",
                        MaxBarWidth = 0,
                        IsVisibleAtLegend = false // Hide from legend since it’s not actual data
                    }
                };
            }
            else
            {
                // Populate Series with actual data if available
                Series = new ISeries[]
                {
                    new ColumnSeries<DateTimePoint>
                    {
                        Values = monthlyIncome.Select(x => new DateTimePoint(x.Date, (double)x.Total)).ToArray(),
                        Name = "Income",
                        Stroke = null,
                        IgnoresBarPosition = true,
                        MaxBarWidth = 60
                    },
                    new ColumnSeries<DateTimePoint>
                    {
                        Values = monthlyExpenses.Select(x => new DateTimePoint(x.Date, (double)x.Total)).ToArray(),
                        Name = "Expenses",
                        Stroke = null,
                        IgnoresBarPosition = true,
                        MaxBarWidth = 30
                    }
                };
            }
        }
    }
}
