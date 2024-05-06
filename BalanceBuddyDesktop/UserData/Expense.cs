using System;

namespace BalanceBuddyDesktop   
{
    public class Expense
    {
        public Guid Id { get; } = Guid.NewGuid();
        public decimal Amount { get; set; }
        public ExpenseCategory Category { get; set; }
        public DateTime Date { get; set; }

        public Expense(decimal amount, ExpenseCategory category, DateTime date)
        {
            Amount = amount;
            Category = category;
            Date = date;
        }

        public override string ToString()
        {
            return $"{Date.ToShortDateString()} - {Category.Name}: {Amount:C}";
        }
    }

}
