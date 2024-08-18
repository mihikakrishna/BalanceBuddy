using System.Linq;
using BalanceBuddyDesktop.Models;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.VisualElements;
using SkiaSharp;

namespace BalanceBuddyDesktop.ViewModels.Charts
{
    public class BankAccountBalanceChartViewModel
    {
        public ISeries[] Series { get; set; }

        public LabelVisual Title { get; set; } =
            new LabelVisual
            {
                Text = "Bank Account Balances",
                TextSize = 25,
                Padding = new LiveChartsCore.Drawing.Padding(15),
                Paint = new SolidColorPaint(SKColors.DarkSlateGray)
            };

        public BankAccountBalanceChartViewModel()
        {
            var bankAccounts = GlobalData.Instance.BankAccounts; // Assuming GlobalData contains a list of bank accounts
            var totalByAccount = bankAccounts
                .Select(account => new
                {
                    AccountName = account.Name,
                    Balance = account.Balance
                });

            Series = totalByAccount.Select(account => new PieSeries<decimal>
            {
                Values = new decimal[] { account.Balance },
                Name = account.AccountName
            }).ToArray();
        }
    }
}
