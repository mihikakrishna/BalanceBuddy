using BalanceBuddyDesktop.ViewModels.Charts;


namespace BalanceBuddyDesktop.ViewModels;

public class ViewExpensesPageViewModel : ViewModelBase
{
    public ExpenseByCategoryChartViewModel ExpenseByCategoryChartViewModel { get; } = new();
    public StackedTransactionChartViewModel StackedTransactionChartViewModel { get; } = new();
    public BankAccountBalanceChartViewModel BankAccountBalanceChartViewModel { get; } = new();
}
