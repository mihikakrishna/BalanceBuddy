using System;
namespace BalanceBuddyDesktop;
public class Account
{
    public Guid Id { get; } = Guid.NewGuid();
    public string Name { get; set; }
    public decimal Balance { get; set; }

    public Account(string name, decimal balance = 0)
    {
        this.Name = name;
        this.Balance = balance;
    }

    public override bool Equals(object? obj)
    {
        return obj is Account source && Name == source.Name;
    }

    public override int GetHashCode()
    {
        return Name.GetHashCode();
    }
}
