using Avalonia.Controls;

namespace BalanceBuddyDesktop
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void TrackSpending_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            this.Content = new TrackSpendingPage();
        }

        private void ViewSpending_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            this.Content = new ViewSpendingPage();
        }
    }
}
