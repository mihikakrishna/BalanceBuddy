using System;
using System.Globalization;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using BalanceBuddyDesktop.Models;
using System.Linq;

namespace BalanceBuddyDesktop.Parsers;

public class AmericanExpressStatementRecord
{
    [Index(0)]
    public DateTime Date { get; set; }

    [Index(1)]
    public string Description { get; set; }

    [Index(2)]
    public decimal Amount { get; set; }
}

public class AmericanExpressParser : IBankStatementParser
{
    public void ParseStatement(Stream csvStream)
    {
        using var reader = new StreamReader(csvStream);
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = ",",
            HasHeaderRecord = true,
            IgnoreBlankLines = true
        });

        var records = csv.GetRecords<AmericanExpressStatementRecord>();
        foreach (var record in records)
        {
            if (record.Amount >= 0)
            {
                GlobalData.Instance.Expenses.Add(new Expense
                {
                    Amount = record.Amount,
                    Date = record.Date,
                    Description = record.Description,
                    Category = GlobalData.Instance.ExpenseCategories.FirstOrDefault(),
                    BankIconPath = "avares://BalanceBuddyDesktop/Assets/Images/AmericanExpressLogo.png"
                });
            }
            else
            {
                GlobalData.Instance.Incomes.Add(new Income
                {
                    Amount = -record.Amount,
                    Date = record.Date,
                    Description = record.Description,
                    Category = GlobalData.Instance.IncomeCategories.FirstOrDefault(),
                    BankIconPath = "avares://BalanceBuddyDesktop/Assets/Images/AmericanExpressLogo.png"
                });
            }
        }
    }
}
