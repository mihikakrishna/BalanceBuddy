using System;
using System.Globalization;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using BalanceBuddyDesktop.Models;
using System.Linq;

namespace BalanceBuddyDesktop.Parsers;

public class BankOfAmericaStatementRecord
{
    [Index(0)]
    public DateTime Date { get; set; }

    [Index(1)]
    public string Description { get; set; }

    [Index(4)]
    public decimal Amount { get; set; }
}

public class BankOfAmericaParser : IBankStatementParser
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

        var records = csv.GetRecords<BankOfAmericaStatementRecord>();
        foreach (var record in records)
        {
            if (record.Amount <= 0)
            {
                GlobalData.Instance.Expenses.Add(new Expense
                {
                    Amount = -record.Amount,
                    Date = record.Date,
                    Description = record.Description,
                    Category = GlobalData.Instance.ExpenseCategories.FirstOrDefault(),
                    BankIconPath = "avares://BalanceBuddyDesktop/Assets/Images/BankOfAmericaLogo.png"
                });
            }
            else
            {
                GlobalData.Instance.Incomes.Add(new Income
                {
                    Amount = record.Amount,
                    Date = record.Date,
                    Description = record.Description,
                    Category = GlobalData.Instance.IncomeCategories.FirstOrDefault(),
                    BankIconPath = "avares://BalanceBuddyDesktop/Assets/Images/BankOfAmericaLogo.png"
                });
            }
        }
    }
}
