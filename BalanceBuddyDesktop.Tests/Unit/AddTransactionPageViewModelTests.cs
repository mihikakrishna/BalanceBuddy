using System;
using System.Collections.ObjectModel;
using NUnit.Framework;
using BalanceBuddyDesktop.ViewModels;
using BalanceBuddyDesktop.Models;

namespace BalanceBuddyDesktop.Tests.Unit
{
    [TestFixture]
    public class AddTransactionPageViewModelTests
    {
        private AddTransactionPageViewModel _viewModel;

        [SetUp]
        public void Setup()
        {
            _viewModel = new AddTransactionPageViewModel();
        }

        [Test]
        public void AddExpense_ShouldIncreaseExpenseCount()
        {
            var expense = new Expense { Amount = 100, Date = DateTime.Now, Description = "Test Expense" };
            _viewModel.NewExpense = expense;

            _viewModel.AddExpenseCommand.Execute(null);

            Assert.That(_viewModel.Expenses.Contains(expense), Is.True);
            Assert.That(_viewModel.Expenses.Count, Is.EqualTo(1));
        }

        [Test]
        public void DeleteExpense_ShouldDecreaseExpenseCount()
        {
            var expense = new Expense { Amount = 100, Date = DateTime.Now, Description = "Test Expense" };
            _viewModel.Expenses.Add(expense);

            _viewModel.DeleteExpenseCommand.Execute(expense);

            Assert.That(_viewModel.Expenses.Contains(expense), Is.False);
        }

        // Similar tests for Income and BankAccount should be added here
    }
}
