using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using BalanceBuddyDesktop.Models;
using BalanceBuddyDesktop.Services;
using BalanceBuddyDesktop.ViewModels;
using BalanceBuddyDesktop.Views;
using System.ComponentModel;
using Avalonia.Controls;
using MsBox.Avalonia;
using MsBox.Avalonia.Dto;
using MsBox.Avalonia.Enums;

namespace BalanceBuddyDesktop
{
    public partial class App : Application
    {
        private readonly DatabaseService _databaseService = DatabaseService.Instance;

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            // Only load data if a DB is currently open.
            // Otherwise, skip loading and let the user Import/Create a DB in the UI.
            if (_databaseService.HasOpenDatabase)
            {
                _databaseService.LoadUserData(GlobalData.Instance);
            }

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                // Remove Avalonia’s default data validation to avoid double validation with MVVM Toolkit
                BindingPlugins.DataValidators.RemoveAt(0);

                desktop.MainWindow = new MainWindow
                {
                    DataContext = new MainWindowViewModel(),
                };

                desktop.MainWindow.Closing += OnMainWindowClosing;
            }

            base.OnFrameworkInitializationCompleted();
        }

        private async void OnMainWindowClosing(object? sender, CancelEventArgs e)
        {
            e.Cancel = true;

            if (GlobalData.Instance.HasUnsavedChanges)
            {
                var messageBoxStandardWindow = MessageBoxManager.GetMessageBoxStandard(new MessageBoxStandardParams
                {
                    ContentTitle = "Unsaved Changes",
                    ContentMessage = "You have unsaved changes. Do you want to save before exiting?",
                    ButtonDefinitions = ButtonEnum.YesNoCancel,
                    Icon = Icon.Warning,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                });

                var result = await messageBoxStandardWindow.ShowAsync();

                if (result == ButtonResult.Yes)
                {
                    _databaseService.SaveUserData(GlobalData.Instance);
                    GlobalData.Instance.HasUnsavedChanges = false;
                    e.Cancel = false;
                }
                else if (result == ButtonResult.No)
                {
                    GlobalData.Instance.HasUnsavedChanges = false;
                    e.Cancel = false;
                }
            }
            else
            {
                e.Cancel = false;
            }
        }
    }
}
