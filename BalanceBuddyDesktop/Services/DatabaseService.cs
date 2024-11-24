using System;

using System.Linq;
using System.Data.SQLite;
using BalanceBuddyDesktop.Models;

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
                IsSelected BOOLEAN,  -- Make IsSelected nullable or remove it if not needed
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
                FOREIGN KEY (CategoryId) REFERENCES IncomeCategories(Id)
            );";

        // Create ExpenseCategories Table
        string createExpenseCategoriesTable = @"
            CREATE TABLE IF NOT EXISTS ExpenseCategories (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL
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
                ('Miscellaneous'), 
                ('Housing'), 
                ('Food'), 
                ('Travel'), 
                ('Utilities'), 
                ('Healthcare'), 
                ('Entertainment');";

            string insertIncomeCategories = @"
                INSERT INTO IncomeCategories (Name) VALUES 
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
                userData.ExpenseCategories.Add(new ExpenseCategory
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    Name = reader["Name"].ToString()
                });
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
                    Description = reader["Description"].ToString()
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
                    Description = reader["Description"].ToString()
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
            var command = new SQLiteCommand("INSERT INTO ExpenseCategories (Name) VALUES (@Name)", connection);
            command.Parameters.AddWithValue("@Name", category.Name);
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

        // Save Expenses, excluding IsSelected field
        foreach (var expense in userData.Expenses)
        {
            var categoryId = GetCategoryId(expense.Category.Name, connection, "ExpenseCategories");
            var command = new SQLiteCommand("INSERT INTO Expenses (Amount, Date, CategoryId, Description) VALUES (@Amount, @Date, @CategoryId, @Description)", connection);
            command.Parameters.AddWithValue("@Amount", expense.Amount);
            command.Parameters.AddWithValue("@Date", expense.Date.ToString("yyyy-MM-dd"));
            command.Parameters.AddWithValue("@CategoryId", categoryId);
            command.Parameters.AddWithValue("@Description", expense.Description);
            command.ExecuteNonQuery();
        }

        // Save Incomes similarly
        foreach (var income in userData.Incomes)
        {
            var categoryId = GetCategoryId(income.Category.Name, connection, "IncomeCategories");
            var command = new SQLiteCommand("INSERT INTO Incomes (Amount, Date, CategoryId, Description) VALUES (@Amount, @Date, @CategoryId, @Description)", connection);
            command.Parameters.AddWithValue("@Amount", income.Amount);
            command.Parameters.AddWithValue("@Date", income.Date.ToString("yyyy-MM-dd"));
            command.Parameters.AddWithValue("@CategoryId", categoryId);
            command.Parameters.AddWithValue("@Description", income.Description);
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
}
