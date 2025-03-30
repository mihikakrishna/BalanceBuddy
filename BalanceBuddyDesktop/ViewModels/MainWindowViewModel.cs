using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media;
using BalanceBuddyDesktop.Models;
using BalanceBuddyDesktop.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MsBox.Avalonia;
using MsBox.Avalonia.Dto;
using MsBox.Avalonia.Enums;

namespace BalanceBuddyDesktop.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase, INotifyPropertyChanged
    {
        [ObservableProperty]
        private bool _isPaneOpen = true;

        [ObservableProperty]
        private ViewModelBase _currentPage = new HomePageViewModel();

        [ObservableProperty]
        private ListItemTemplate _selectedListItem;

        partial void OnSelectedListItemChanged(ListItemTemplate? value)
        {
            if (value is null) return;
            var instance = Activator.CreateInstance(value.ModelType);
            if (instance == null) return;
            CurrentPage = (ViewModelBase)instance;
        }

        public ObservableCollection<ListItemTemplate> Items { get; } = new()
        {
            new ListItemTemplate(typeof(HomePageViewModel), "home_regular"),
            new ListItemTemplate(typeof(ParseStatementPageViewModel), "text_column_three_regular"),
            new ListItemTemplate(typeof(AddTransactionPageViewModel), "add_circle_regular"),
            new ListItemTemplate(typeof(ViewExpensesPageViewModel), "book_pulse_regular"),
            new ListItemTemplate(typeof(SettingsPageViewModel), "wrench_regular")
        };

        [RelayCommand]
        private void TriggerPane()
        {
            IsPaneOpen = !IsPaneOpen;
        }

        // Create a new empty database at a user-specified path:
        [RelayCommand]
        private async Task CreateNewDatabaseAsync()
        {
            var desktop = App.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
            var mainWindow = desktop?.MainWindow;

            if (mainWindow != null)
            {
                var saveFileDialog = new SaveFileDialog
                {
                    Title = "Create New Database",
                    InitialFileName = "new_balancebuddy.db",
                    Filters = new List<FileDialogFilter>
                    {
                        new FileDialogFilter { Name = "SQLite Database", Extensions = { "db" } }
                    }
                };

                var resultPath = await saveFileDialog.ShowAsync(mainWindow);
                if (!string.IsNullOrWhiteSpace(resultPath))
                {
                    // Create a brand new DB at that path
                    DatabaseService.Instance.CreateNewDatabase(resultPath);
                    DatabaseService.Instance.LoadUserData(GlobalData.Instance);

                    await ShowMessageAsync("New Database Created",
                        $"A new database has been created at:\n{resultPath}");
                }
            }
        }

        // Import an existing .db file:
        [RelayCommand]
        private async Task ImportDatabaseAsync()
        {
            var desktop = App.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
            var mainWindow = desktop?.MainWindow;

            if (mainWindow != null)
            {
                await DatabaseService.Instance.ImportDatabaseAsync(mainWindow);

                // After import (open) is done, load the data into our global data:
                if (DatabaseService.Instance.HasOpenDatabase)
                {
                    DatabaseService.Instance.LoadUserData(GlobalData.Instance);
                    mainWindow.DataContext = new MainWindowViewModel();
                }
            }
        }

        // Export the currently open database
        [RelayCommand]
        private async Task ExportDatabaseAsync()
        {
            var mainWindow = App.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop
                                ? desktop.MainWindow
                                : null;
            if (mainWindow != null)
            {
                SaveAll();  // Save changes before exporting
                await DatabaseService.Instance.ExportDatabaseAsync(mainWindow);
            }
        }

        // Saves the user data to the currently open database file (overwriting it)
        [RelayCommand]
        public async void SaveAll()
        {
            if (!DatabaseService.Instance.HasOpenDatabase)
            {
                await ShowMessageAsync("No Open Database",
                    "No database is currently open. Please create or import a database first.");
                return;
            }

            DatabaseService.Instance.SaveUserData(GlobalData.Instance);
            GlobalData.Instance.HasUnsavedChanges = false;

            await ShowMessageAsync("Save All",
                "Your changes have been saved successfully!");
        }

        private async Task ShowMessageAsync(string title, string content)
        {
            var messageBoxStandardWindow = MessageBoxManager.GetMessageBoxStandard(new MessageBoxStandardParams
            {
                ContentTitle = title,
                ContentMessage = content,
                Icon = Icon.Success,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            });
            await messageBoxStandardWindow.ShowAsync();
        }
    }

    public class ListItemTemplate
    {
        public ListItemTemplate(Type type, string iconKey)
        {
            ModelType = type;
            Label = type.Name.Replace("PageViewModel", "");

            Application.Current!.TryFindResource(iconKey, out var res);
            ListItemIcon = (StreamGeometry)res!;
        }

        public string Label { get; }
        public Type ModelType { get; }
        public StreamGeometry ListItemIcon { get; }
    }
}