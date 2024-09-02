using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity.Migrations.Model;
using System.Linq;
using Avalonia.Media;
using BalanceBuddyDesktop.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;


namespace BalanceBuddyDesktop.ViewModels.Charts
{
    public partial class StackedTransactionChartViewModel : ViewModelBase, INotifyPropertyChanged
    {
        [ObservableProperty]
        private ObservableCollection<int> _availableYears  = new ObservableCollection<int>();

        [ObservableProperty]
        private int _selectedYear = new();

        [ObservableProperty]
        private ISeries[] _series = new ISeries[] { };

        public Axis[] XAxes { get; set; } = new[]
        {
            new DateTimeAxis(TimeSpan.FromDays(30), date => date.ToString("MMM yyyy"))
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
            LoadAvailableYears();
            SelectedYear = DateTime.Now.Year;
            UpdateSeries();
        }

        private void LoadAvailableYears()
        {
            var expenseYears = GlobalData.Instance.Expenses.Select(e => e.Date.Year);
            var incomeYears = GlobalData.Instance.Incomes.Select(i => i.Date.Year);

            var allYears = expenseYears.Concat(incomeYears).Distinct().OrderBy(y => y);
            foreach (var year in allYears)
            {
                AvailableYears.Add(year);
            }
        }

        public void UpdateSeries()
        {
            var monthlyExpenses = GlobalData.Instance.Expenses
                .Where(e => e.Date.Year == SelectedYear)
                .GroupBy(e => new { e.Date.Year, e.Date.Month })
                .Select(g => new
                {
                    Date = new DateTime(g.Key.Year, g.Key.Month, 1),
                    Total = g.Sum(e => e.Amount)
                })
                .OrderBy(m => m.Date);

            var monthlyIncome = GlobalData.Instance.Incomes
                .Where(i => i.Date.Year == SelectedYear)
                .GroupBy(i => new { i.Date.Year, i.Date.Month })
                .Select(g => new
                {
                    Date = new DateTime(g.Key.Year, g.Key.Month, 1),
                    Total = g.Sum(i => i.Amount)
                })
                .OrderBy(m => m.Date);

            Series = new ISeries[]
            {
            new ColumnSeries<DateTimePoint>
            {
                Values = monthlyIncome.Select(x => new DateTimePoint(x.Date, (double)x.Total)).ToArray(),
                Name = "Income",
                MaxBarWidth = 60,
                IgnoresBarPosition = true,
                Fill = new SolidColorPaint(SKColors.SkyBlue)
            },
            new ColumnSeries<DateTimePoint>
            {
                Values = monthlyExpenses.Select(x => new DateTimePoint(x.Date, (double)x.Total)).ToArray(),
                Name = "Expenses",
                MaxBarWidth = 30,
                IgnoresBarPosition = true,
                Fill = new SolidColorPaint(SKColors.Coral)
            }
            };
        }
    }
}
