using System;
using System.ComponentModel;
using System.Linq;
using BalanceBuddyDesktop.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.VisualElements;
using SkiaSharp;

namespace BalanceBuddyDesktop.ViewModels.Charts
{
    public partial class ExpenseByCategoryChartViewModel : ViewModelBase, INotifyPropertyChanged
    {
        [ObservableProperty]
        private DateTimeOffset? _selectedMonth;

        [ObservableProperty]
        private ISeries[] _series = Array.Empty<ISeries>();

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
            _selectedMonth = DateTimeOffset.Now;
            UpdateSeries();
        }

        partial void OnSelectedMonthChanged(DateTimeOffset? value)
        {
            UpdateSeries();
        }

        public void UpdateSeries()
        {
            if (SelectedMonth.HasValue)
            {
                var selectedDate = SelectedMonth.Value.DateTime;
                var month = selectedDate.Month;
                var year = selectedDate.Year;

                if (GlobalData.Instance?.Expenses != null)
                {
                    var expenses = GlobalData.Instance.Expenses
                        .Where(e => e.Date.Year == year && e.Date.Month == month);

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
                else
                {
                    Series = Array.Empty<ISeries>();
                }
            }
            else
            {
                Series = Array.Empty<ISeries>();
            }
        }
    }
}
