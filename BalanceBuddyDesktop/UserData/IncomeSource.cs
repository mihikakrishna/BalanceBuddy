namespace BalanceBuddyDesktop;
public class IncomeSource
{
    public string  Name { get; set; }

    public decimal Balance { get; set; }

    public IncomeSource(string name, decimal balance = 0)
    {
        this.Name = name;
        this.Balance = balance;
    }

    public override bool Equals(object? obj)
    {
        return obj is IncomeSource source && Name == source.Name;
    }

    public override int GetHashCode()
    {
        return Name.GetHashCode();
    }
}
