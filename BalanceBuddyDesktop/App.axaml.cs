using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

namespace BalanceBuddyDesktop
{
    public partial class App : Application
    {
        public static UserData? UserDataInstance { get; private set; }

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow();
            }

            UserDataInstance = new UserData();
            base.OnFrameworkInitializationCompleted();
        }
    }
}