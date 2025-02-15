using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using BalanceBuddyDesktop.Models;
using MsBox.Avalonia;
using MsBox.Avalonia.Dto;
using MsBox.Avalonia.Enums;

namespace BalanceBuddyDesktop.Services;

public class DatabaseService
{
    private static DatabaseService instance = null;
    private static readonly object padlock = new object();

    private const string ConnectionString = "Data Source=balancebuddy.db;Version=3;";

    public void CreateDatabase()
    {
        using var connection = new SQLiteConnection(ConnectionString);
        connection.Open();
    }

    public void CreateTables()
    {
        using var connection = new SQLiteConnection(ConnectionString);
        connection.Open();

        // Create BankAccounts Table
        string createBankAccountsTable = @"
            CREATE TABLE IF NOT EXISTS BankAccounts (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                Balance DECIMAL NOT NULL,
                Description TEXT
            );";

        // Create Expenses Table with IsSelected as NULLABLE
        string createExpensesTable = @"
            CREATE TABLE IF NOT EXISTS Expenses (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Amount DECIMAL NOT NULL,
                Date TEXT NOT NULL,
                CategoryId INTEGER,
                Description TEXT,
                IsSelected BOOLEAN,
                BankIconPath TEXT,
                FOREIGN KEY (CategoryId) REFERENCES ExpenseCategories(Id)
            );";

        // Create Incomes Table
        string createIncomesTable = @"
            CREATE TABLE IF NOT EXISTS Incomes (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Amount DECIMAL NOT NULL,
                Date TEXT NOT NULL,
                CategoryId INTEGER,
                Description TEXT,
                BankIconPath TEXT,
                FOREIGN KEY (CategoryId) REFERENCES IncomeCategories(Id)
            );";

        // Create ExpenseCategories Table with Budget column
        string createExpenseCategoriesTable = @"
            CREATE TABLE IF NOT EXISTS ExpenseCategories (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                Budget REAL
            );";


        // Create IncomeCategories Table
        string createIncomeCategoriesTable = @"
            CREATE TABLE IF NOT EXISTS IncomeCategories (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL
            );";

        // Execute the SQL commands to create tables
        ExecuteNonQuery(createBankAccountsTable, connection);
        ExecuteNonQuery(createExpensesTable, connection);
        ExecuteNonQuery(createIncomesTable, connection);
        ExecuteNonQuery(createExpenseCategoriesTable, connection);
        ExecuteNonQuery(createIncomeCategoriesTable, connection);
    }

    private void ExecuteNonQuery(string commandText, SQLiteConnection connection)
    {
        using var command = new SQLiteCommand(commandText, connection);
        command.ExecuteNonQuery();
    }

    public void InitializeCategories()
    {
        using var connection = new SQLiteConnection(ConnectionString);
        connection.Open();

        string checkCategories = "SELECT COUNT(*) FROM ExpenseCategories;";
        using var checkCommand = new SQLiteCommand(checkCategories, connection);
        var count = Convert.ToInt32(checkCommand.ExecuteScalar());

        if (count == 0)
        {
            string insertExpenseCategories = @"
                INSERT INTO ExpenseCategories (Name) VALUES 
                ('Unreviewed'),
                ('Miscellaneous'), 
                ('Housing'), 
                ('Food'), 
                ('Travel'), 
                ('Utilities'), 
                ('Healthcare'), 
                ('Entertainment');";

            string insertIncomeCategories = @"
                INSERT INTO IncomeCategories (Name) VALUES 
                ('Unreviewed'),
                ('Other'), 
                ('Job');";

            ExecuteNonQuery(insertExpenseCategories, connection);
            ExecuteNonQuery(insertIncomeCategories, connection);
        }
    }

    private DatabaseService()
    {
        CreateDatabase();
        CreateTables();
        InitializeCategories();
    }

    public static DatabaseService Instance
    {
        get
        {
            lock (padlock)
            {
                if (instance == null)
                {
                    instance = new DatabaseService();
                }
                return instance;
            }
        }
    }

    // Method to load data from the database into UserData
    public void LoadUserData(UserData userData)
    {
        using var connection = new SQLiteConnection(ConnectionString);
        connection.Open();

        // Clear existing data
        userData.BankAccounts.Clear();
        userData.Expenses.Clear();
        userData.Incomes.Clear();
        userData.ExpenseCategories.Clear();
        userData.IncomeCategories.Clear();

        // Load ExpenseCategories
        string queryExpenseCategories = "SELECT * FROM ExpenseCategories;";
        using (var command = new SQLiteCommand(queryExpenseCategories, connection))
        using (var reader = command.ExecuteReader())
        {
            while (reader.Read())
            {
                var expenseCategory = new ExpenseCategory
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    Name = reader["Name"].ToString()
                };

                // Check if the Budget field is not null in the database.
                int budgetOrdinal = reader.GetOrdinal("Budget");
                if (!reader.IsDBNull(budgetOrdinal))
                {
                    expenseCategory.Budget = reader.GetDecimal(budgetOrdinal);
                }
                else
                {
                    expenseCategory.Budget = null;
                }

                userData.ExpenseCategories.Add(expenseCategory);
            }
        }

        // Load IncomeCategories
        string queryIncomeCategories = "SELECT * FROM IncomeCategories;";
        using (var command = new SQLiteCommand(queryIncomeCategories, connection))
        using (var reader = command.ExecuteReader())
        {
            while (reader.Read())
            {
                userData.IncomeCategories.Add(new IncomeCategory
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    Name = reader["Name"].ToString()
                });
            }
        }

        // Load BankAccounts
        string queryBankAccounts = "SELECT * FROM BankAccounts;";
        using (var command = new SQLiteCommand(queryBankAccounts, connection))
        using (var reader = command.ExecuteReader())
        {
            while (reader.Read())
            {
                userData.BankAccounts.Add(new BankAccount
                {
                    Name = reader["Name"].ToString(),
                    Balance = Convert.ToDecimal(reader["Balance"]),
                    Description = reader["Description"].ToString()
                });
            }
        }

        // Load Expenses
        string queryExpenses = "SELECT * FROM Expenses;";
        using (var command = new SQLiteCommand(queryExpenses, connection))
        using (var reader = command.ExecuteReader())
        {
            while (reader.Read())
            {
                userData.Expenses.Add(new Expense
                {
                    Amount = Convert.ToDecimal(reader["Amount"]),
                    Date = DateTime.Parse(reader["Date"].ToString()),
                    Category = userData.ExpenseCategories.FirstOrDefault(c => c.Id == Convert.ToInt32(reader["CategoryId"])),
                    Description = reader["Description"].ToString(),
                    BankIconPath = reader["BankIconPath"]?.ToString()
                });
            }
        }

        // Load Incomes
        string queryIncomes = "SELECT * FROM Incomes;";
        using (var command = new SQLiteCommand(queryIncomes, connection))
        using (var reader = command.ExecuteReader())
        {
            while (reader.Read())
            {
                userData.Incomes.Add(new Income
                {
                    Amount = Convert.ToDecimal(reader["Amount"]),
                    Date = DateTime.Parse(reader["Date"].ToString()),
                    Category = userData.IncomeCategories.FirstOrDefault(c => c.Id == Convert.ToInt32(reader["CategoryId"])),
                    Description = reader["Description"].ToString(),
                    BankIconPath = reader["BankIconPath"]?.ToString()
                });
            }
        }
    }


    public void SaveUserData(UserData userData)
    {
        using var connection = new SQLiteConnection(ConnectionString);
        connection.Open();

        // Clear existing data to avoid duplication; this could be optimized further as needed
        ExecuteNonQuery("DELETE FROM BankAccounts;", connection);
        ExecuteNonQuery("DELETE FROM Expenses;", connection);
        ExecuteNonQuery("DELETE FROM Incomes;", connection);
        ExecuteNonQuery("DELETE FROM ExpenseCategories;", connection);
        ExecuteNonQuery("DELETE FROM IncomeCategories;", connection);

        // Save ExpenseCategories
        foreach (var category in userData.ExpenseCategories)
        {
            var command = new SQLiteCommand("INSERT INTO ExpenseCategories (Name, Budget) VALUES (@Name, @Budget)", connection);
            command.Parameters.AddWithValue("@Name", category.Name);
            command.Parameters.AddWithValue("@Budget", category.Budget);
            command.ExecuteNonQuery();
        }

        // Save IncomeCategories
        foreach (var category in userData.IncomeCategories)
        {
            var command = new SQLiteCommand("INSERT INTO IncomeCategories (Name) VALUES (@Name)", connection);
            command.Parameters.AddWithValue("@Name", category.Name);
            command.ExecuteNonQuery();
        }

        // Save BankAccounts
        foreach (var account in userData.BankAccounts)
        {
            var command = new SQLiteCommand("INSERT INTO BankAccounts (Name, Balance, Description) VALUES (@Name, @Balance, @Description)", connection);
            command.Parameters.AddWithValue("@Name", account.Name);
            command.Parameters.AddWithValue("@Balance", account.Balance);
            command.Parameters.AddWithValue("@Description", account.Description);
            command.ExecuteNonQuery();
        }

        // Save Expenses
        foreach (var expense in userData.Expenses)
        {
            if (expense.Category == null)
            {
                expense.Category = userData.ExpenseCategories.FirstOrDefault();
            }

            var categoryId = GetCategoryId(expense.Category.Name, connection, "ExpenseCategories");
            var command = new SQLiteCommand(
                "INSERT INTO Expenses (Amount, Date, CategoryId, Description, BankIconPath) " +
                "VALUES (@Amount, @Date, @CategoryId, @Description, @BankIconPath)",
                connection);

            command.Parameters.AddWithValue("@Amount", expense.Amount);
            command.Parameters.AddWithValue("@Date", expense.Date.ToString("yyyy-MM-dd"));
            command.Parameters.AddWithValue("@CategoryId", categoryId);
            command.Parameters.AddWithValue("@Description", expense.Description ?? "");
            command.Parameters.AddWithValue("@BankIconPath", expense.BankIconPath ?? "");

            command.ExecuteNonQuery();
        }

        // Save Incomes
        foreach (var income in userData.Incomes)
        {
            if (income.Category == null)
            {
                income.Category = userData.IncomeCategories.FirstOrDefault();
            }

            var categoryId = GetCategoryId(income.Category.Name, connection, "IncomeCategories");
            var command = new SQLiteCommand(
                "INSERT INTO Incomes (Amount, Date, CategoryId, Description, BankIconPath) " +
                "VALUES (@Amount, @Date, @CategoryId, @Description, @BankIconPath)",
                connection);

            command.Parameters.AddWithValue("@Amount", income.Amount);
            command.Parameters.AddWithValue("@Date", income.Date.ToString("yyyy-MM-dd"));
            command.Parameters.AddWithValue("@CategoryId", categoryId);
            command.Parameters.AddWithValue("@Description", income.Description ?? "");
            command.Parameters.AddWithValue("@BankIconPath", income.BankIconPath ?? "");

            command.ExecuteNonQuery();
        }

    }

    private string GetCategoryName(int categoryId, SQLiteConnection connection, string tableName)
    {
        string query = $"SELECT Name FROM {tableName} WHERE Id = @Id;";
        using var command = new SQLiteCommand(query, connection);
        command.Parameters.AddWithValue("@Id", categoryId);
        return command.ExecuteScalar()?.ToString() ?? "Unknown";
    }

    private int GetCategoryId(string categoryName, SQLiteConnection connection, string tableName)
    {
        string query = $"SELECT Id FROM {tableName} WHERE Name = @Name;";
        using var command = new SQLiteCommand(query, connection);
        command.Parameters.AddWithValue("@Name", categoryName);
        var result = command.ExecuteScalar();
        return result != null ? Convert.ToInt32(result) : 0;
    }

    public async Task ExportDatabaseAsync(Window parent)
    {
        var saveFileDialog = new SaveFileDialog
        {
            Title = "Export Database",
            InitialFileName = "balancebuddy_backup.db",
            Filters = new List<FileDialogFilter>
        {
            new FileDialogFilter { Name = "SQLite Database", Extensions = { "db" } }
        }
        };

        string destinationPath = await saveFileDialog.ShowAsync(parent);
        if (!string.IsNullOrWhiteSpace(destinationPath))
        {
            try
            {
                File.Copy("balancebuddy.db", destinationPath, overwrite: true);

                var messageBoxStandardWindow = MessageBoxManager.GetMessageBoxStandard(new MessageBoxStandardParams
                {
                    ContentTitle = "Export Successful",
                    ContentMessage = $"Your database has been exported successfully to:\n{destinationPath}",
                    Icon = Icon.Success,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                });
                await messageBoxStandardWindow.ShowAsync();
            }
            catch (Exception ex)
            {
                var messageBoxStandardWindow = MessageBoxManager.GetMessageBoxStandard(new MessageBoxStandardParams
                {
                    ContentTitle = "Export Failed",
                    ContentMessage = $"An error occurred during export:\n{ex.Message}",
                    Icon = Icon.Error,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                });
                await messageBoxStandardWindow.ShowAsync();
            }
        }
    }

    public async Task ImportDatabaseAsync(Window parent)
    {
        var openFileDialog = new OpenFileDialog
        {
            Title = "Import Database",
            AllowMultiple = false,
            Filters = new List<FileDialogFilter>
        {
            new FileDialogFilter { Name = "SQLite Database", Extensions = { "db" } }
        }
        };

        string[] result = await openFileDialog.ShowAsync(parent);
        if (result != null && result.Length > 0)
        {
            string selectedFilePath = result[0];
            try
            {
                File.Copy(selectedFilePath, "balancebuddy.db", overwrite: true);

                var messageBoxStandardWindow = MessageBoxManager.GetMessageBoxStandard(new MessageBoxStandardParams
                {
                    ContentTitle = "Import Successful",
                    ContentMessage = $"Your database has been imported successfully from:\n{selectedFilePath}",
                    Icon = Icon.Success,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                });
                await messageBoxStandardWindow.ShowAsync();
            }
            catch (Exception ex)
            {
                var messageBoxStandardWindow = MessageBoxManager.GetMessageBoxStandard(new MessageBoxStandardParams
                {
                    ContentTitle = "Import Failed",
                    ContentMessage = $"An error occurred during import:\n{ex.Message}",
                    Icon = Icon.Error,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                });
                await messageBoxStandardWindow.ShowAsync();
            }
        }
    }
}
