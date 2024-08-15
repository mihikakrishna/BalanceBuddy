using System;
using System.Linq;
using BalanceBuddyDesktop.Models;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;

namespace BalanceBuddyDesktop.ViewModels.Charts
{
    public class StackedTransactionChartViewModel
    {
        public ISeries[] Series { get; set; }

        public Axis[] XAxes { get; set; } = new[]
        {
            new DateTimeAxis(TimeSpan.FromDays(1), date => date.ToString("MMMM dd"))
        };

        public Axis[] YAxes { get; set; } = new[]
        {
            new Axis
            {
                Labeler = value => value.ToString("C"), // Formats the currency
                Name = "Amount"
            }
        };

        public StackedTransactionChartViewModel()
        {
            var monthlyExpenses = GlobalData.Instance.Expenses
                .GroupBy(e => e.Date.Month)
                .Select(g => new { Month = g.Key, Total = g.Sum(e => e.Amount) })
                .OrderBy(m => m.Month);

            var monthlyIncome = GlobalData.Instance.Incomes
                .GroupBy(i => i.Date.Month)
                .Select(g => new { Month = g.Key, Total = g.Sum(i => i.Amount) })
                .OrderBy(m => m.Month);

            Series =
            [
                new ColumnSeries<decimal>
                {
                    Values = monthlyIncome.Select(x => (decimal)x.Total).ToArray(),
                    Name = "Income",
                    Stroke = null,
                    MaxBarWidth = 60,
                    IgnoresBarPosition = true
                },
                new ColumnSeries<decimal>
                {
                    Values = monthlyExpenses.Select(x => (decimal)x.Total).ToArray(),
                    Name = "Expenses",
                    Stroke = null,
                    MaxBarWidth = 30,
                    IgnoresBarPosition = true
                }
            ];
        }
    }
}
