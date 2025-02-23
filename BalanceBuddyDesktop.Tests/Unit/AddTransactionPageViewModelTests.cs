using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using NUnit.Framework;
using BalanceBuddyDesktop.Models;
using BalanceBuddyDesktop.Services;
using BalanceBuddyDesktop.ViewModels;
using NUnit.Framework.Legacy;

namespace BalanceBuddyDesktop.Tests.Unit
{
    [TestFixture]
    public class AddTransactionPageViewModelTests
    {
        [SetUp]
        public void Setup()
        {
            // Reset GlobalData.Instance for testing.
            GlobalData.Instance.Expenses = new List<Expense>
            {
                new Expense
                {
                    Amount = 100,
                    Date = new DateTime(2023, 5, 10),
                    Description = "Lunch at cafe",
                    Category = new ExpenseCategory{ Name = "Food" }
                },
                new Expense
                {
                    Amount = 200,
                    Date = new DateTime(2023, 5, 15),
                    Description = "Train ticket",
                    Category = new ExpenseCategory{ Name = "Travel" }
                },
                new Expense
                {
                    Amount = 50,
                    Date = new DateTime(2023, 4, 20),
                    Description = "Stationery",
                    Category = new ExpenseCategory{ Name = "Misc" }
                }
            };
            GlobalData.Instance.Incomes = new List<Income>
            {
                new Income
                {
                    Amount = 500,
                    Date = new DateTime(2023, 5, 12),
                    Description = "Monthly Salary",
                    Category = new IncomeCategory { Name = "Salary" }
                },
                new Income
                {
                    Amount = 300,
                    Date = new DateTime(2023, 4, 25),
                    Description = "Project Bonus",
                    Category = new IncomeCategory { Name = "Bonus" }
                }
            };
            GlobalData.Instance.BankAccounts = new List<BankAccount>
            {
                new BankAccount { Name = "Checking", Balance = 1000 },
                new BankAccount { Name = "Savings", Balance = 5000 }
            };
            GlobalData.Instance.ExpenseCategories = new List<ExpenseCategory>
            {
                new ExpenseCategory{Name = "Food" },
                new ExpenseCategory{ Name = "Travel" }
            };
            GlobalData.Instance.IncomeCategories = new List<IncomeCategory>
            {
                new IncomeCategory { Name = "Salary" },
                new IncomeCategory { Name = "Bonus" }
            };
            GlobalData.Instance.HasUnsavedChanges = false;

            _viewModel = new AddTransactionPageViewModel();
            // Ensure that no filtering hides test data.
            _viewModel.SelectedMonth = "";
            _viewModel.RefreshExpenses();
        }

        private AddTransactionPageViewModel _viewModel;

        #region Add Commands

        [Test]
        public void AddExpense_ShouldIncreaseExpenseCount_AndResetNewExpense()
        {
            int initialCount = _viewModel.Expenses.Count;
            var newExp = new Expense { Amount = 150, Date = DateTime.Today, Description = "Test Expense", Category = new ExpenseCategory { Name = "Food" } };
            _viewModel.NewExpense = newExp;

            _viewModel.AddExpenseCommand.Execute(null);

            Assert.That(_viewModel.Expenses.Contains(newExp), Is.True, "New expense should be in the list.");
            Assert.That(_viewModel.Expenses.Count, Is.EqualTo(initialCount + 1), "Expense count should increase by 1.");
            Assert.That(_viewModel.NewExpense.Amount, Is.EqualTo(0), "NewExpense should be reset (Amount==0).");
        }

        [Test]
        public void AddIncome_ShouldIncreaseIncomeCount_AndResetNewIncome()
        {
            int initialCount = _viewModel.Incomes.Count;
            var newInc = new Income { Amount = 250, Date = DateTime.Today, Description = "Test Income", Category = new IncomeCategory { Name = "Salary" }};
            _viewModel.NewIncome = newInc;

            _viewModel.AddIncomeCommand.Execute(null);

            Assert.That(_viewModel.Incomes.Contains(newInc), Is.True, "New income should be in the list.");
            Assert.That(_viewModel.Incomes.Count, Is.EqualTo(initialCount + 1));
            Assert.That(_viewModel.NewIncome.Amount, Is.EqualTo(0), "NewIncome should be reset.");
        }

        [Test]
        public void AddBankAccount_ShouldIncreaseBankAccountCount_AndResetNewBankAccount()
        {
            int initialCount = _viewModel.BankAccounts.Count;
            var newAcc = new BankAccount { Name = "TestAccount", Balance = 1234, Description = "Test Bank" };
            _viewModel.NewBankAccount = newAcc;

            _viewModel.AddBankAccountCommand.Execute(null);

            Assert.That(_viewModel.BankAccounts.Contains(newAcc), Is.True, "New bank account should be in the list.");
            Assert.That(_viewModel.BankAccounts.Count, Is.EqualTo(initialCount + 1));
            Assert.That(_viewModel.NewBankAccount.Name, Is.Null, "NewBankAccount should be reset (Name is empty).");
        }

        #endregion

        #region Delete Commands

        [Test]
        public void DeleteExpense_ShouldRemoveExpenseFromCollection()
        {
            var expense = _viewModel.Expenses.First();
            int initialCount = _viewModel.Expenses.Count;

            _viewModel.DeleteExpenseCommand.Execute(expense);

            Assert.That(_viewModel.Expenses.Contains(expense), Is.False, "Expense should have been removed.");
            Assert.That(_viewModel.Expenses.Count, Is.EqualTo(initialCount - 1));
        }

        [Test]
        public void DeleteIncome_ShouldRemoveIncomeFromCollection()
        {
            var income = _viewModel.Incomes.First();
            int initialCount = _viewModel.Incomes.Count;

            _viewModel.DeleteIncomeCommand.Execute(income);

            Assert.That(_viewModel.Incomes.Contains(income), Is.False, "Income should have been removed.");
            Assert.That(_viewModel.Incomes.Count, Is.EqualTo(initialCount - 1));
        }

        [Test]
        public void DeleteBankAccount_ShouldRemoveBankAccountFromCollection()
        {
            var account = _viewModel.BankAccounts.First();
            int initialCount = _viewModel.BankAccounts.Count;

            _viewModel.DeleteBankAccountCommand.Execute(account);

            Assert.That(_viewModel.BankAccounts.Contains(account), Is.False, "Bank account should have been removed.");
            Assert.That(_viewModel.BankAccounts.Count, Is.EqualTo(initialCount - 1));
        }

        [Test]
        public void DeleteSelectedExpenses_ShouldRemoveAllSelectedExpenses()
        {
            var expense1 = _viewModel.Expenses.First();
            var expense2 = _viewModel.Expenses.Skip(1).First();
            _viewModel.SelectedExpenses = new List<Expense> { expense1, expense2 };
            int initialCount = _viewModel.Expenses.Count;

            _viewModel.DeleteSelectedExpensesCommand.Execute(null);

            Assert.That(_viewModel.Expenses.Contains(expense1), Is.False);
            Assert.That(_viewModel.Expenses.Contains(expense2), Is.False);
            Assert.That(_viewModel.Expenses.Count, Is.EqualTo(initialCount - 2));
            Assert.That(GlobalData.Instance.HasUnsavedChanges, Is.True);
        }

        [Test]
        public void DeleteSelectedIncomes_ShouldRemoveAllSelectedIncomes()
        {
            var income1 = _viewModel.Incomes.First();
            _viewModel.SelectedIncomes = new List<Income> { income1 };
            int initialCount = _viewModel.Incomes.Count;

            _viewModel.DeleteSelectedIncomesCommand.Execute(null);

            Assert.That(_viewModel.Incomes.Contains(income1), Is.False);
            Assert.That(_viewModel.Incomes.Count, Is.EqualTo(initialCount - 1));
            Assert.That(GlobalData.Instance.HasUnsavedChanges, Is.True);
        }

        [Test]
        public void DeleteSelectedBankAccounts_ShouldRemoveAllSelectedBankAccounts()
        {
            var account = _viewModel.BankAccounts.First();
            _viewModel.SelectedBankAccounts = new List<BankAccount> { account };
            int initialCount = _viewModel.BankAccounts.Count;

            _viewModel.DeleteSelectedBankAccountsCommand.Execute(null);

            Assert.That(_viewModel.BankAccounts.Contains(account), Is.False);
            Assert.That(_viewModel.BankAccounts.Count, Is.EqualTo(initialCount - 1));
            Assert.That(GlobalData.Instance.HasUnsavedChanges, Is.True);
        }

        #endregion

        #region Refresh and Filter Commands

        [Test]
        public void RefreshExpenses_ShouldApplyMonthFilterAndSort()
        {
            _viewModel.SelectedMonth = "May";
            _viewModel.ExpenseSortAscending = true;

            _viewModel.RefreshExpenses();

            foreach (var expense in _viewModel.Expenses)
            {
                Assert.That(expense.Date.Month, Is.EqualTo(5));
                Assert.That(expense.Date.Year, Is.EqualTo(DateTime.Now.Year));
            }

            var expected = GlobalData.Instance.Expenses
                .Where(e => e.Date.Month == 5 && e.Date.Year == DateTime.Now.Year)
                .OrderBy<Expense, long>(e => _viewModel.ExpenseSortAscending ? e.Date.Ticks : -e.Date.Ticks)
                .ThenBy<Expense, decimal>(e => _viewModel.ExpenseSortAscending ? e.Amount : -e.Amount)
                .Select(e => e.Date)
                .ToList();
            Assert.That(_viewModel.Expenses.Select(e => e.Date).ToList(), Is.EqualTo(expected));
        }

        [Test]
        public void RefreshExpenses_ShouldApplyDateRangeFilter()
        {
            _viewModel.SelectedMonth = "";
            _viewModel.SelectedExpenseDates.Clear();
            DateTime start = new DateTime(2023, 4, 1);
            DateTime end = new DateTime(2023, 4, 30);
            _viewModel.SelectedExpenseDates.Add(start);
            _viewModel.SelectedExpenseDates.Add(end);

            _viewModel.RefreshExpenses();

            foreach (var expense in _viewModel.Expenses)
            {
                Assert.That(expense.Date, Is.InRange(start, end));
            }
        }

        [Test]
        public void RefreshIncomes_ShouldApplyMonthFilterAndSort()
        {
            _viewModel.SelectedMonth = "May";
            _viewModel.IncomeSortAscending = true;

            _viewModel.RefreshIncomes();

            foreach (var income in _viewModel.Incomes)
            {
                Assert.That(income.Date.Month, Is.EqualTo(5));
                Assert.That(income.Date.Year, Is.EqualTo(DateTime.Now.Year));
            }

            var expected = GlobalData.Instance.Incomes
                .Where(i => i.Date.Month == 5 && i.Date.Year == DateTime.Now.Year)
                .OrderBy<Income, long>(i => _viewModel.IncomeSortAscending ? i.Date.Ticks : -i.Date.Ticks)
                .ThenBy<Income, decimal>(i => _viewModel.IncomeSortAscending ? i.Amount : -i.Amount)
                .Select(i => i.Date)
                .ToList();
            Assert.That(_viewModel.Incomes.Select(i => i.Date).ToList(), Is.EqualTo(expected));
        }

        [Test]
        public void RefreshBankAccounts_ShouldSortBasedOnBalanceAndName()
        {
            _viewModel.BankAccountSortAscending = true;
            _viewModel.RefreshBankAccounts();
            var expectedAsc = GlobalData.Instance.BankAccounts
                .OrderBy<BankAccount, decimal>(b => _viewModel.BankAccountSortAscending ? b.Balance : -b.Balance)
                .ThenBy<BankAccount, string>(b => _viewModel.BankAccountSortAscending ? b.Name : string.Concat(b.Name.Reverse()))
                .Select(b => b.Balance)
                .ToList();
            Assert.That(_viewModel.BankAccounts.Select(b => b.Balance).ToList(), Is.EqualTo(expectedAsc));

            _viewModel.BankAccountSortAscending = false;
            _viewModel.RefreshBankAccounts();
            var expectedDesc = GlobalData.Instance.BankAccounts
                .OrderByDescending<BankAccount, decimal>(b => b.Balance)
                .ThenByDescending<BankAccount, string>(b => b.Name)
                .Select(b => b.Balance)
                .ToList();
            Assert.That(_viewModel.BankAccounts.Select(b => b.Balance).ToList(), Is.EqualTo(expectedDesc));
        }

        [Test]
        public void FilterExpensesByMonth_ShouldUpdateExpenses()
        {
            _viewModel.SelectedMonth = "May";
            _viewModel.FilterExpensesByMonthCommand.Execute(null);

            foreach (var expense in _viewModel.Expenses)
            {
                Assert.That(expense.Date.Month, Is.EqualTo(5));
                Assert.That(expense.Date.Year, Is.EqualTo(DateTime.Now.Year));
            }
        }

        [Test]
        public void FilterIncomesByMonth_ShouldUpdateIncomes()
        {
            _viewModel.SelectedMonth = "May";
            _viewModel.FilterIncomesByMonthCommand.Execute(null);

            foreach (var income in _viewModel.Incomes)
            {
                Assert.That(income.Date.Month, Is.EqualTo(5));
                Assert.That(income.Date.Year, Is.EqualTo(DateTime.Now.Year));
            }
        }

        #endregion

        #region Undo/Redo Commands

        [Test]
        public void UndoExpense_ShouldRefreshExpenses()
        {
            _viewModel.UndoExpenseCommand.Execute(null);
            var expected = GlobalData.Instance.Expenses
                .Where(e => string.IsNullOrEmpty(_viewModel.SelectedMonth) ||
                            (e.Date.Month == DateTime.ParseExact(_viewModel.SelectedMonth, "MMMM", CultureInfo.InvariantCulture).Month
                             && e.Date.Year == DateTime.Now.Year))
                .OrderBy<Expense, long>(e => _viewModel.ExpenseSortAscending ? e.Date.Ticks : -e.Date.Ticks)
                .ThenBy<Expense, decimal>(e => _viewModel.ExpenseSortAscending ? e.Amount : -e.Amount)
                .Select(e => e.Date)
                .ToList();
            Assert.That(_viewModel.Expenses.Select(e => e.Date).ToList(), Is.EqualTo(expected));
        }

        [Test]
        public void RedoExpense_ShouldRefreshExpenses()
        {
            _viewModel.RedoExpenseCommand.Execute(null);
            var expected = GlobalData.Instance.Expenses
                .Where(e => string.IsNullOrEmpty(_viewModel.SelectedMonth) ||
                            (e.Date.Month == DateTime.ParseExact(_viewModel.SelectedMonth, "MMMM", CultureInfo.InvariantCulture).Month
                             && e.Date.Year == DateTime.Now.Year))
                .OrderBy<Expense, long>(e => _viewModel.ExpenseSortAscending ? e.Date.Ticks : -e.Date.Ticks)
                .ThenBy<Expense, decimal>(e => _viewModel.ExpenseSortAscending ? e.Amount : -e.Amount)
                .Select(e => e.Date)
                .ToList();
            Assert.That(_viewModel.Expenses.Select(e => e.Date).ToList(), Is.EqualTo(expected));
        }

        [Test]
        public void UndoIncome_ShouldRefreshIncomes()
        {
            _viewModel.UndoIncomeCommand.Execute(null);
            var expected = GlobalData.Instance.Incomes
                .Where(i => string.IsNullOrEmpty(_viewModel.SelectedMonth) ||
                            (i.Date.Month == DateTime.ParseExact(_viewModel.SelectedMonth, "MMMM", CultureInfo.InvariantCulture).Month
                             && i.Date.Year == DateTime.Now.Year))
                .OrderBy<Income, long>(i => _viewModel.IncomeSortAscending ? i.Date.Ticks : -i.Date.Ticks)
                .ThenBy<Income, decimal>(i => _viewModel.IncomeSortAscending ? i.Amount : -i.Amount)
                .Select(i => i.Date)
                .ToList();
            Assert.That(_viewModel.Incomes.Select(i => i.Date).ToList(), Is.EqualTo(expected));
        }

        [Test]
        public void RedoIncome_ShouldRefreshIncomes()
        {
            _viewModel.RedoIncomeCommand.Execute(null);
            var expected = GlobalData.Instance.Incomes
                .Where(i => string.IsNullOrEmpty(_viewModel.SelectedMonth) ||
                            (i.Date.Month == DateTime.ParseExact(_viewModel.SelectedMonth, "MMMM", CultureInfo.InvariantCulture).Month
                             && i.Date.Year == DateTime.Now.Year))
                .OrderBy<Income, long>(i => _viewModel.IncomeSortAscending ? i.Date.Ticks : -i.Date.Ticks)
                .ThenBy<Income, decimal>(i => _viewModel.IncomeSortAscending ? i.Amount : -i.Amount)
                .Select(i => i.Date)
                .ToList();
            Assert.That(_viewModel.Incomes.Select(i => i.Date).ToList(), Is.EqualTo(expected));
        }

        [Test]
        public void UndoBankAccount_ShouldRefreshBankAccounts()
        {
            _viewModel.UndoBankAccountCommand.Execute(null);
            var expected = GlobalData.Instance.BankAccounts
                .OrderBy<BankAccount, decimal>(b => _viewModel.BankAccountSortAscending ? b.Balance : -b.Balance)
                .ThenBy<BankAccount, string>(b => _viewModel.BankAccountSortAscending ? b.Name : string.Concat(b.Name.Reverse()))
                .Select(b => b.Balance)
                .ToList();
            Assert.That(_viewModel.BankAccounts.Select(b => b.Balance).ToList(), Is.EqualTo(expected));
        }

        [Test]
        public void RedoBankAccount_ShouldRefreshBankAccounts()
        {
            _viewModel.RedoBankAccountCommand.Execute(null);
            var expected = GlobalData.Instance.BankAccounts
                .OrderBy<BankAccount, decimal>(b => _viewModel.BankAccountSortAscending ? b.Balance : -b.Balance)
                .ThenBy<BankAccount, string>(b => _viewModel.BankAccountSortAscending ? b.Name : string.Concat(b.Name.Reverse()))
                .Select(b => b.Balance)
                .ToList();
            Assert.That(_viewModel.BankAccounts.Select(b => b.Balance).ToList(), Is.EqualTo(expected));
        }

        #endregion

        #region Property Change and GlobalData Unsaved Flag

        [Test]
        public void ItemPropertyChanged_ShouldSetHasUnsavedChanges()
        {
            GlobalData.Instance.HasUnsavedChanges = false;
            var expense = _viewModel.Expenses.First();
            // Instead of invoking the event (which isn't allowed externally), we change a property value.
            decimal original = expense.Amount;
            expense.Amount = original + 1;
            Assert.That(GlobalData.Instance.HasUnsavedChanges, Is.True, "Changing a property should mark GlobalData as unsaved.");
        }

        #endregion
    }
}
