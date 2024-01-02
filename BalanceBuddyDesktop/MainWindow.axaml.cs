using Avalonia.Controls;
namespace BalanceBuddyDesktop;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        var homePage = new HomePage();
        homePage.RequestNavigate += NavigateTo;
        MainContent.Content = homePage;
    }

    private void NavigateTo(UserControl newPage)
    {
        MainContent.Content = newPage;
    }
}