using System;
using System.Linq;
using NUnit.Framework;
using BalanceBuddyDesktop.ViewModels;
using BalanceBuddyDesktop.Models;
using System.Collections.Generic;

namespace BalanceBuddyDesktop.Tests.Functional
{
    [TestFixture]
    public class AddTransactionPageViewModelFunctionalTests
    {
        [SetUp]
        public void Setup()
        {
            // Reset GlobalData collections before each test.
            GlobalData.Instance.Expenses = new List<Expense>();
            GlobalData.Instance.Incomes = new List<Income>();
            GlobalData.Instance.BankAccounts = new List<BankAccount>();
        }

        [Test]
        public void RefreshExpenses_SortsExpensesInDescendingOrder()
        {
            // Arrange: create expenses with unsorted dates.
            var expense1 = new Expense { Date = new DateTime(2023, 1, 1) };
            var expense2 = new Expense { Date = new DateTime(2023, 3, 1) };
            var expense3 = new Expense { Date = new DateTime(2023, 2, 1) };

            GlobalData.Instance.Expenses.AddRange(new[] { expense1, expense2, expense3 });

            // Act: initialize the view model and execute the refresh command.
            var viewModel = new AddTransactionPageViewModel();
            viewModel.RefreshExpensesCommand.Execute(null);

            // Assert: verify expenses are sorted descending (newest first).
            Assert.That(viewModel.Expenses.First(), Is.EqualTo(expense2));
            Assert.That(viewModel.Expenses.Skip(1).First(), Is.EqualTo(expense3));
            Assert.That(viewModel.Expenses.Last(), Is.EqualTo(expense1));
        }

        [Test]
        public void FilterExpenses_SortsFilteredExpensesInDescendingOrder()
        {
            // Arrange: create expenses with unsorted dates.
            var expense1 = new Expense { Date = new DateTime(2023, 1, 1) };
            var expense2 = new Expense { Date = new DateTime(2023, 3, 1) };
            var expense3 = new Expense { Date = new DateTime(2023, 2, 1) };

            GlobalData.Instance.Expenses.AddRange(new[] { expense1, expense2, expense3 });

            // Act: initialize the view model, set filter dates covering all expenses, and execute the filter command.
            var viewModel = new AddTransactionPageViewModel();
            viewModel.SelectedExpenseDates.Add(new DateTime(2023, 1, 1));
            viewModel.SelectedExpenseDates.Add(new DateTime(2023, 3, 1));
            viewModel.FilterExpensesCommand.Execute(null);

            // Assert: verify the filtered expenses are sorted descending by date.
            Assert.That(viewModel.Expenses.First(), Is.EqualTo(expense2));
            Assert.That(viewModel.Expenses.Skip(1).First(), Is.EqualTo(expense3));
            Assert.That(viewModel.Expenses.Last(), Is.EqualTo(expense1));
        }

        [Test]
        public void RefreshIncomes_SortsIncomesInDescendingOrder()
        {
            // Arrange: create incomes with unsorted dates.
            var income1 = new Income { Date = new DateTime(2023, 1, 1) };
            var income2 = new Income { Date = new DateTime(2023, 3, 1) };
            var income3 = new Income { Date = new DateTime(2023, 2, 1) };

            GlobalData.Instance.Incomes.AddRange(new[] { income1, income2, income3 });

            // Act: initialize the view model and execute the refresh command for incomes.
            var viewModel = new AddTransactionPageViewModel();
            viewModel.RefreshIncomesCommand.Execute(null);

            // Assert: verify incomes are sorted descending (newest first).
            Assert.That(viewModel.Incomes.First(), Is.EqualTo(income2));
            Assert.That(viewModel.Incomes.Skip(1).First(), Is.EqualTo(income3));
            Assert.That(viewModel.Incomes.Last(), Is.EqualTo(income1));
        }

        [Test]
        public void FilterIncomes_SortsFilteredIncomesInDescendingOrder()
        {
            // Arrange: create incomes with unsorted dates.
            var income1 = new Income { Date = new DateTime(2023, 1, 1) };
            var income2 = new Income { Date = new DateTime(2023, 3, 1) };
            var income3 = new Income { Date = new DateTime(2023, 2, 1) };

            GlobalData.Instance.Incomes.AddRange(new[] { income1, income2, income3 });

            // Act: initialize the view model, set filter dates covering all incomes, and execute the filter command.
            var viewModel = new AddTransactionPageViewModel();
            viewModel.SelectedIncomeDates.Add(new DateTime(2023, 1, 1));
            viewModel.SelectedIncomeDates.Add(new DateTime(2023, 3, 1));
            viewModel.FilterIncomesCommand.Execute(null);

            // Assert: verify the filtered incomes are sorted descending by date.
            Assert.That(viewModel.Incomes.First(), Is.EqualTo(income2));
            Assert.That(viewModel.Incomes.Skip(1).First(), Is.EqualTo(income3));
            Assert.That(viewModel.Incomes.Last(), Is.EqualTo(income1));
        }

        [Test]
        public void DeleteSelectedExpense_RemovesOnlyTheSelectedRow()
        {
            // Arrange: Add multiple expenses to GlobalData.
            var expense1 = new Expense { Date = new DateTime(2023, 1, 1) };
            var expense2 = new Expense { Date = new DateTime(2023, 2, 1) };
            var expense3 = new Expense { Date = new DateTime(2023, 3, 1) };

            GlobalData.Instance.Expenses.AddRange(new[] { expense1, expense2, expense3 });

            // Initialize the view model which loads the GlobalData.
            var viewModel = new AddTransactionPageViewModel();

            // Pre-condition: the datagrid (view model) should have all three expenses.
            Assert.That(viewModel.Expenses.Count, Is.EqualTo(3));

            // Act: Simulate selecting one row (expense2) for deletion.
            viewModel.SelectedExpenses = new List<Expense> { expense2 };
            viewModel.DeleteSelectedExpensesCommand.Execute(null);

            // Assert: Check that only expense2 was deleted.
            Assert.That(viewModel.Expenses.Count, Is.EqualTo(2));
            Assert.That(viewModel.Expenses, Does.Contain(expense1));
            Assert.That(viewModel.Expenses, Does.Contain(expense3));
            Assert.That(viewModel.Expenses, Does.Not.Contain(expense2));

            // Also verify that GlobalData has been updated.
            Assert.That(GlobalData.Instance.Expenses, Does.Contain(expense1));
            Assert.That(GlobalData.Instance.Expenses, Does.Contain(expense3));
            Assert.That(GlobalData.Instance.Expenses, Does.Not.Contain(expense2));
        }
    }
}
