using System;

namespace BalanceBuddyDesktop
{
    public class ExpenseCategory
    {
        public Guid Id { get; } = Guid.NewGuid();
        public string Name { get; set; }
        public decimal Budget { get; set; }

        public ExpenseCategory(string name, decimal budget = 0)
        {
            Name = name;
            Budget = budget;
        }

        public override bool Equals(object? obj)
        {
            return obj is ExpenseCategory category && Name == category.Name;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}
