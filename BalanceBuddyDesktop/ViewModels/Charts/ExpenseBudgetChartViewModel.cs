using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using BalanceBuddyDesktop.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Drawing;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Sketches;
using SkiaSharp;

namespace BalanceBuddyDesktop.ViewModels.Charts
{
    public partial class ExpenseBudgetChartViewModel : ViewModelBase, INotifyPropertyChanged
    {
        [ObservableProperty]
        private ObservableCollection<int> _availableYears = new ObservableCollection<int>();

        [ObservableProperty]
        private int _selectedYear = new();

        [ObservableProperty]
        private ISeries[] _series = new ISeries[] { };

        public Axis[] XAxes { get; set; } = new[]
        {
            new Axis
            {
                Name = "Months",
                Labels = Enumerable.Range(1, 12)
                    .Select(m => new DateTime(2000, m, 1).ToString("MMM"))
                    .ToArray(),
                LabelsRotation = 15,
                SeparatorsPaint = new SolidColorPaint { Color = SKColors.Gray }
            }
        };

        public Axis[] YAxes { get; set; } = new[]
        {
            new Axis
            {
                Name = "Categories",
                Labels = GlobalData.Instance.ExpenseCategories.Select(c => c.Name).ToArray()
            }
        };

        static ExpenseBudgetChartViewModel()
        {
            LiveCharts.Configure(config =>
            {
                config.HasMap<double[]>((point, index) =>
                {
                    var x = point[0];
                    var y = point[1];
                    var weight = point[2];
                    return new Coordinate(x, y, weight);
                });
            });
        }

        public ExpenseBudgetChartViewModel()
        {
            LoadAvailableYears();
            SelectedYear = DateTime.Now.Year;
            UpdateSeries();
        }

        private void LoadAvailableYears()
        {
            var expenseYears = GlobalData.Instance.Expenses.Select(e => e.Date.Year);
            var allYears = expenseYears.Distinct().OrderBy(y => y);

            foreach (var year in allYears)
            {
                AvailableYears.Add(year);
            }
        }

        public void UpdateSeries()
        {
            var categories = GlobalData.Instance.ExpenseCategories;
            var heatmapValues = new List<double[]>();

            foreach (var category in categories)
            {
                for (int month = 1; month <= 12; month++)
                {
                    var totalSpent = GlobalData.Instance.Expenses
                        .Where(e => e.Category == category && e.Date.Year == SelectedYear && e.Date.Month == month)
                        .Sum(e => e.Amount);

                    double percentage = (category.Budget.HasValue && category.Budget.Value > 0)
                    ? (double)(totalSpent / category.Budget.Value)
                    : 0;


                    heatmapValues.Add(new double[]
                    {
                        month - 1,
                        categories.IndexOf(category),
                        percentage > 1 ? 1 : percentage
                    });
                }
            }

            Series = new ISeries[]
            {
                new HeatSeries<double[]>
                {
                    Values = heatmapValues.ToArray(),
                    HeatMap = new[]
                    {
                        new LvcColor(141, 213, 127),
                        new LvcColor(123, 190, 127),
                        new LvcColor(248, 215, 109),
                        new LvcColor(255, 182, 75),
                        new LvcColor(255, 105, 97)
                    },
                    TooltipLabelFormatter = point =>
                    {
                        int monthIndex = (int)Math.Round(point.Model[0]);
                        int categoryIndex = (int)Math.Round(point.Model[1]);
                        var month = XAxes[0].Labels[monthIndex];
                        var category = YAxes[0].Labels[categoryIndex];
                        var totalSpent = GlobalData.Instance.Expenses
                            .Where(e => e.Category.Name == category && e.Date.Year == SelectedYear && e.Date.Month == monthIndex + 1)
                            .Sum(e => e.Amount);

                        return $"{month} - {category}: Total Spent ${totalSpent:0.##}";
                    }
                }
            };
        }
    }
}
