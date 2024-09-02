using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using BalanceBuddyDesktop.Models;
using BalanceBuddyDesktop.Services;
using BalanceBuddyDesktop.ViewModels;
using BalanceBuddyDesktop.Views;

namespace BalanceBuddyDesktop
{
    public partial class App : Application
    {
        private readonly DatabaseService _databaseService = new DatabaseService();

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            _databaseService.SetupDatabase();

            _databaseService.LoadUserData(GlobalData.Instance);

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                // Line below is needed to remove Avalonia data validation.
                // Without this line, you will get duplicate validations from both Avalonia and CT
                BindingPlugins.DataValidators.RemoveAt(0);

                desktop.MainWindow = new MainWindow
                {
                    DataContext = new MainWindowViewModel(),
                };

                desktop.Exit += OnExit;
            }

            base.OnFrameworkInitializationCompleted();
        }

        private void OnExit(object? sender, ControlledApplicationLifetimeExitEventArgs e)
        {
            _databaseService.SaveUserData(GlobalData.Instance);
        }
    }
}
