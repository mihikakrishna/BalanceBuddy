using Avalonia.Controls;
namespace BalanceBuddyDesktop;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        var homePage = new HomePage();
        homePage.RequestNavigate += OnRequestNavigate;
        MainContent.Content = homePage;
    }

    private void OnRequestNavigate(UserControl newPage)
    {
        if (MainContent.Content is INavigable currentNavigablePage)
        {
            currentNavigablePage.RequestNavigate -= OnRequestNavigate;
        }

        MainContent.Content = newPage;

        if (newPage is INavigable newNavigablePage)
        {
            newNavigablePage.RequestNavigate += OnRequestNavigate;
        }
    }
}