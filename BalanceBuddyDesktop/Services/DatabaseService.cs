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

namespace BalanceBuddyDesktop.Services
{
    public class DatabaseService
    {
        private static DatabaseService instance = null;
        private static readonly object padlock = new object();
        private string _databaseFilePath;
        private string ConnectionString => $"Data Source={_databaseFilePath};Version=3;";

        private DatabaseService()
        {
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

        /// <summary>
        /// Returns true if _databaseFilePath is set and the file exists.
        /// </summary>
        public bool HasOpenDatabase =>
            !string.IsNullOrEmpty(_databaseFilePath) && File.Exists(_databaseFilePath);

        /// <summary>
        /// Creates a brand-new SQLite database file at the specified path,
        /// runs table creation, seeds categories, etc. Sets this file as "open."
        /// </summary>
        /// <param name="newDbPath">File path for the new .db file.</param>
        public void CreateNewDatabase(string newDbPath)
        {
            _databaseFilePath = newDbPath;

            // Create a blank file if it doesn’t exist yet.
            if (!File.Exists(_databaseFilePath))
            {
                SQLiteConnection.CreateFile(_databaseFilePath);
            }

            // Now create all required tables, seed categories, etc.
            CreateTables();
            InitializeCategories();
        }

        /// <summary>
        /// Opens (or “attaches”) an existing database file,
        /// runs table creation if needed, seeds categories, etc.
        /// </summary>
        /// <param name="existingDbPath">Path to an existing .db file.</param>
        /// <exception cref="FileNotFoundException"></exception>
        public void OpenDatabase(string existingDbPath)
        {
            if (!File.Exists(existingDbPath))
                throw new FileNotFoundException("Could not find the specified database file.", existingDbPath);

            _databaseFilePath = existingDbPath;

            // Optionally: create missing tables if the user has an older DB, seed categories, etc.
            CreateTables();
            InitializeCategories();
        }

        /// <summary>
        /// Create all necessary tables if they do not exist.
        /// </summary>
        private void CreateTables()
        {
            using var connection = new SQLiteConnection(ConnectionString);
            connection.Open();

            string createBankAccountsTable = @"
                CREATE TABLE IF NOT EXISTS BankAccounts (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    Balance DECIMAL NOT NULL,
                    Description TEXT
                );";

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

            string createExpenseCategoriesTable = @"
                CREATE TABLE IF NOT EXISTS ExpenseCategories (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    Budget REAL
                );";

            string createIncomeCategoriesTable = @"
                CREATE TABLE IF NOT EXISTS IncomeCategories (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL
                );";

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

        /// <summary>
        /// Seeds default categories if none exist.
        /// </summary>
        private void InitializeCategories()
        {
            using var connection = new SQLiteConnection(ConnectionString);
            connection.Open();

            // Check if any expense categories exist yet.
            string checkCategories = "SELECT COUNT(*) FROM ExpenseCategories;";
            using var checkCommand = new SQLiteCommand(checkCategories, connection);
            var count = Convert.ToInt32(checkCommand.ExecuteScalar());

            // If none, insert default categories.
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

        /// <summary>
        /// Loads all user data from the currently open database into the provided UserData object.
        /// </summary>
        /// <param name="userData"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public void LoadUserData(UserData userData)
        {
            if (!HasOpenDatabase)
                throw new InvalidOperationException("No database is currently open. Import or create a new database first.");

            using var connection = new SQLiteConnection(ConnectionString);
            connection.Open();

            // Clear existing data before reloading
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

                    // If the Budget column is not null, store it.
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

            // Add “Unreviewed” if missing in expense categories.
            if (!userData.ExpenseCategories.Any(c =>
                    c.Name.Equals("Unreviewed", StringComparison.OrdinalIgnoreCase)))
            {
                userData.ExpenseCategories.Add(new ExpenseCategory { Name = "Unreviewed" });
            }

            // Add “Unreviewed” if missing in income categories.
            if (!userData.IncomeCategories.Any(c =>
                    c.Name.Equals("Unreviewed", StringComparison.OrdinalIgnoreCase)))
            {
                userData.IncomeCategories.Add(new IncomeCategory { Name = "Unreviewed" });
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
                        Category = userData.ExpenseCategories.FirstOrDefault(
                            c => c.Id == Convert.ToInt32(reader["CategoryId"])),
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
                        Category = userData.IncomeCategories.FirstOrDefault(
                            c => c.Id == Convert.ToInt32(reader["CategoryId"])),
                        Description = reader["Description"].ToString(),
                        BankIconPath = reader["BankIconPath"]?.ToString()
                    });
                }
            }
        }

        /// <summary>
        /// Saves all user data into the currently open database file, replacing existing data.
        /// </summary>
        /// <param name="userData"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public void SaveUserData(UserData userData)
        {
            if (!HasOpenDatabase)
                throw new InvalidOperationException("No database is currently open. Cannot save.");

            using var connection = new SQLiteConnection(ConnectionString);
            connection.Open();

            // Clear existing data to avoid duplication.
            ExecuteNonQuery("DELETE FROM BankAccounts;", connection);
            ExecuteNonQuery("DELETE FROM Expenses;", connection);
            ExecuteNonQuery("DELETE FROM Incomes;", connection);
            ExecuteNonQuery("DELETE FROM ExpenseCategories;", connection);
            ExecuteNonQuery("DELETE FROM IncomeCategories;", connection);

            // Save ExpenseCategories
            foreach (var category in userData.ExpenseCategories)
            {
                var command = new SQLiteCommand(
                    "INSERT INTO ExpenseCategories (Name, Budget) VALUES (@Name, @Budget)",
                    connection
                );
                command.Parameters.AddWithValue("@Name", category.Name);
                command.Parameters.AddWithValue("@Budget", (object?)category.Budget ?? DBNull.Value);
                command.ExecuteNonQuery();
            }

            // Save IncomeCategories
            foreach (var category in userData.IncomeCategories)
            {
                var command = new SQLiteCommand(
                    "INSERT INTO IncomeCategories (Name) VALUES (@Name)",
                    connection
                );
                command.Parameters.AddWithValue("@Name", category.Name);
                command.ExecuteNonQuery();
            }

            // Helper method to fetch category ID from DB by name,
            // after re-inserting categories above.
            int GetCategoryId(string catName, SQLiteConnection conn, string table)
            {
                string query = $"SELECT Id FROM {table} WHERE Name = @Name LIMIT 1;";
                using var cmd = new SQLiteCommand(query, conn);
                cmd.Parameters.AddWithValue("@Name", catName);
                var result = cmd.ExecuteScalar();
                return result != null ? Convert.ToInt32(result) : 0;
            }

            // Save BankAccounts
            foreach (var account in userData.BankAccounts)
            {
                var command = new SQLiteCommand(
                    "INSERT INTO BankAccounts (Name, Balance, Description) VALUES (@Name, @Balance, @Description)",
                    connection
                );
                command.Parameters.AddWithValue("@Name", account.Name);
                command.Parameters.AddWithValue("@Balance", account.Balance);
                command.Parameters.AddWithValue("@Description", account.Description);
                command.ExecuteNonQuery();
            }

            // Save Expenses
            foreach (var expense in userData.Expenses)
            {
                // If category is null, default to the first expense category or something similar.
                if (expense.Category == null)
                {
                    expense.Category = userData.ExpenseCategories.FirstOrDefault();
                }

                var categoryId = GetCategoryId(expense.Category.Name, connection, "ExpenseCategories");
                var command = new SQLiteCommand(
                    "INSERT INTO Expenses (Amount, Date, CategoryId, Description, BankIconPath) " +
                    "VALUES (@Amount, @Date, @CategoryId, @Description, @BankIconPath)",
                    connection
                );

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
                    connection
                );

                command.Parameters.AddWithValue("@Amount", income.Amount);
                command.Parameters.AddWithValue("@Date", income.Date.ToString("yyyy-MM-dd"));
                command.Parameters.AddWithValue("@CategoryId", categoryId);
                command.Parameters.AddWithValue("@Description", income.Description ?? "");
                command.Parameters.AddWithValue("@BankIconPath", income.BankIconPath ?? "");
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Exports (copies) the currently open database file to a user-chosen destination.
        /// </summary>
        public async Task ExportDatabaseAsync(Window parent)
        {
            if (!HasOpenDatabase)
            {
                await ShowErrorMessage(parent, "No active database to export.");
                return;
            }

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
                    File.Copy(_databaseFilePath, destinationPath, overwrite: true);

                    await ShowSuccessMessage(parent, "Export Successful",
                        $"Your database has been exported successfully to:\n{destinationPath}");
                }
                catch (Exception ex)
                {
                    await ShowErrorMessage(parent, $"An error occurred during export:\n{ex.Message}");
                }
            }
        }

        /// <summary>
        /// Opens a user-chosen database file from disk by calling OpenDatabase(...).
        /// </summary>
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
                try
                {
                    // Instead of overwriting a fixed file, we just “open” the selected file.
                    string selectedFilePath = result[0];
                    OpenDatabase(selectedFilePath);

                    await ShowSuccessMessage(parent, "Import Successful",
                        $"Your database has been imported/opened successfully from:\n{selectedFilePath}");
                }
                catch (Exception ex)
                {
                    await ShowErrorMessage(parent, $"An error occurred during import:\n{ex.Message}");
                }
            }
        }

        private async Task ShowErrorMessage(Window parent, string message)
        {
            var messageBoxStandardWindow = MessageBoxManager.GetMessageBoxStandard(new MessageBoxStandardParams
            {
                ContentTitle = "Error",
                ContentMessage = message,
                Icon = Icon.Error,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            });
            await messageBoxStandardWindow.ShowAsync();
        }

        private async Task ShowSuccessMessage(Window parent, string title, string message)
        {
            var messageBoxStandardWindow = MessageBoxManager.GetMessageBoxStandard(new MessageBoxStandardParams
            {
                ContentTitle = title,
                ContentMessage = message,
                Icon = Icon.Success,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            });
            await messageBoxStandardWindow.ShowAsync();
        }
    }
}
