using System;
using System.Globalization;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using BalanceBuddyDesktop.Models;
using System.Linq;

namespace BalanceBuddyDesktop.Parsers;

public class WellsFargoStatementRecord
{
    [Index(0)]
    public DateTime Date { get; set; }

    [Index(1)]
    public decimal Amount { get; set; }

    [Index(4)]
    public string Description { get; set; }
}

public class WellsFargoParser : IBankStatementParser
{
    public void ParseStatement(Stream csvStream)
    {
        using var reader = new StreamReader(csvStream);
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = ",",
            HasHeaderRecord = false,
            IgnoreBlankLines = true
        });

        var records = csv.GetRecords<WellsFargoStatementRecord>();
        foreach (var record in records)
        {
            if (record.Amount <= 0)
            {
                GlobalData.Instance.Expenses.Add(new Expense
                {
                    Amount = -record.Amount,
                    Date = record.Date,
                    Description = record.Description,
                    Category = GlobalData.Instance.ExpenseCategories.FirstOrDefault()
                });
            }
            else
            {
                GlobalData.Instance.Incomes.Add(new Income
                {
                    Amount = record.Amount,
                    Date = record.Date,
                    Description = record.Description,
                    Category = GlobalData.Instance.IncomeCategories.FirstOrDefault()
                });
            }
        }
    }
}
