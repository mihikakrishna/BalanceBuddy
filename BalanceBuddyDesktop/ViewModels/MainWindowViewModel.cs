using System;
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


namespace BalanceBuddyDesktop.ViewModels;

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

    [RelayCommand]
    private async Task ExportDatabaseAsync()
    {
        var mainWindow = App.Current?.ApplicationLifetime is Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop
                            ? desktop.MainWindow
                            : null;
        if (mainWindow != null)
        {
            SaveAll();
            await DatabaseService.Instance.ExportDatabaseAsync(mainWindow);
        }
    }

    [RelayCommand]
    private async Task ImportDatabaseAsync()
    {
        var desktop = App.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
        var mainWindow = desktop?.MainWindow;

        if (mainWindow != null)
        {
            await DatabaseService.Instance.ImportDatabaseAsync(mainWindow);
            DatabaseService.Instance.LoadUserData(GlobalData.Instance);
            mainWindow.DataContext = new MainWindowViewModel();
        }
    }

    [RelayCommand]
    public async void SaveAll()
    {
        DatabaseService.Instance.SaveUserData(GlobalData.Instance);
        GlobalData.Instance.HasUnsavedChanges = false;
        var messageBoxStandardWindow = MessageBoxManager.GetMessageBoxStandard(new MessageBoxStandardParams
        {
            ContentTitle = "Save All",
            ContentMessage = "Your changes have been saved successfully!",
            Icon = Icon.Success,
            WindowStartupLocation = WindowStartupLocation.CenterOwner
        });
        var result = await messageBoxStandardWindow.ShowAsync();
    }

    [RelayCommand]
    public void Undo()
    {
        TransactionService.Undo();
        RefreshCollections();
    }

    [RelayCommand]
    public void Redo()
    {
        TransactionService.Redo();
        RefreshCollections();
    }

    private void RefreshCollections()
    {
        if (CurrentPage is IRefreshable refreshable)
        {
            refreshable.Refresh();
        }
    }
}

public class ListItemTemplate
{
    public ListItemTemplate(Type type, string iconKey) 
    { 
        ModelType = type;
        Label = type.Name.Replace("PageViewModel", "");

        Application.Current!.TryFindResource(iconKey, out var res);
        ListItemIcon = (StreamGeometry)res;
    }   
    public string Label { get; }
    public Type ModelType { get; }
    public StreamGeometry ListItemIcon { get; }
}