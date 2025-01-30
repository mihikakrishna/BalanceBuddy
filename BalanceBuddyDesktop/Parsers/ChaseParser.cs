using System;
using System.Globalization;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using BalanceBuddyDesktop.Models;
using System.Linq;

namespace BalanceBuddyDesktop.Parsers;

public class ChaseStatementRecord
{
    [Index(1)]
    public DateTime Date { get; set; }

    [Index(2)]
    public string Description { get; set; }

    [Index(5)]
    public decimal Amount { get; set; }
}

public class ChaseParser : IBankStatementParser
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

        var records = csv.GetRecords<ChaseStatementRecord>();
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
                    BankIconPath = "avares://BalanceBuddyDesktop/Assets/Images/ChaseLogo.png"
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
                    BankIconPath = "avares://BalanceBuddyDesktop/Assets/Images/ChaseLogo.png"
                });
            }
        }
    }
}
