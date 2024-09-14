using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BalanceBuddyDesktop.Models;

namespace BalanceBuddyDesktop.Models
{
    public abstract class Transaction
    {
        public string Id { get; set; }
        public bool IsSelected { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string FormattedDate => Date.ToString("d");
        public string Description { get; set; }
    }
}

public class Expense : Transaction
{
    public ExpenseCategory Category { get; set; }
}

public class Income : Transaction
{
    public IncomeCategory Category { get; set; }
}