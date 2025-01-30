using System;
using System.Globalization;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using BalanceBuddyDesktop.Models;
using System.Linq;

namespace BalanceBuddyDesktop.Parsers;

public class CapitalOneCreditStatementRecord
{
    [Index(1)]
    public DateTime Date { get; set; }

    [Index(3)]
    public string Description { get; set; }

    [Index(5)]
    public decimal? Debit { get; set; }

    [Index(6)]
    public decimal? Credit { get; set; }
}

public class CapitalOneCreditParser : IBankStatementParser
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

        var records = csv.GetRecords<CapitalOneCreditStatementRecord>();
        foreach (var record in records)
        {
            if (record.Debit.HasValue && record.Debit >= 0)
            {
                GlobalData.Instance.Expenses.Add(new Expense
                {
                    Amount = record.Debit.Value,
                    Date = record.Date,
                    Description = record.Description,
                    Category = GlobalData.Instance.ExpenseCategories.FirstOrDefault(),
                    BankIconPath = "avares://BalanceBuddyDesktop/Assets/Images/CapitalOneCreditLogo.jpg"
                });
            }
            else if (record.Credit.HasValue && record.Credit > 0)
            {
                GlobalData.Instance.Incomes.Add(new Income
                {
                    Amount = record.Credit.Value,
                    Date = record.Date,
                    Description = record.Description,
                    Category = GlobalData.Instance.IncomeCategories.FirstOrDefault(),
                    BankIconPath = "avares://BalanceBuddyDesktop/Assets/Images/CapitalOneCreditLogo.jpg"
                });
            }
        }
    }
}
