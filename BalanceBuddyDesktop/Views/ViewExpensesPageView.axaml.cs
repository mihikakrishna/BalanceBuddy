using Avalonia.Controls;
using BalanceBuddyDesktop.ViewModels;

namespace BalanceBuddyDesktop.Views
{
    public partial class ViewExpensesPageView : UserControl
    {
        public ViewExpensesPageView()
        {
            InitializeComponent();
        }

        private void OnYearSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext is ViewExpensesPageViewModel viewModel)
            {
                viewModel.StackedTransactionChartViewModel.UpdateSeries();
            }
        }
    }
}
