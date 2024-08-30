using System;
using System.Globalization;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using BalanceBuddyDesktop.Models;
using System.Linq;

namespace BalanceBuddyDesktop.Parsers;

public class CapitalOneStatementRecord
{
    [Index(1)]
    public string Description { get; set; }

    [Index(2)]
    public DateTime Date { get; set; }

    [Index(3)]
    public string TransactionType { get; set; }

    [Index(4)]
    public decimal Amount { get; set; }
}

public class CapitalOneParser : IBankStatementParser
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

        var records = csv.GetRecords<CapitalOneStatementRecord>();
        foreach (var record in records)
        {
            if (record.TransactionType.Equals("Debit"))
            {
                GlobalData.Instance.Expenses.Add(new Expense
                {
                    Amount = record.Amount,
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
