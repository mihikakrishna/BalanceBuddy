using System;
using System.Globalization;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using BalanceBuddyDesktop.Models;
using System.Linq;
using BalanceBuddyDesktop.Services;

namespace BalanceBuddyDesktop.Parsers;

public class CapitalOneSavingsStatementRecord
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

public class CapitalOneSavingsParser : IBankStatementParser
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

        var records = csv.GetRecords<CapitalOneSavingsStatementRecord>();
        foreach (var record in records)
        {
            if (record.TransactionType.Equals("Debit"))
            {
                var expense = new Expense
                {
                    Amount = record.Amount,
                    Date = record.Date,
                    Description = record.Description,
                    Category = GlobalData.Instance.ExpenseCategories.FirstOrDefault(),
                    BankIconPath = "avares://BalanceBuddyDesktop/Assets/Images/CapitalOneSavingsLogo.jpg"
                };
                TransactionService.AddExpense(expense);
            }
            else
            {
                var income = new Income
                {
                    Amount = record.Amount,
                    Date = record.Date,
                    Description = record.Description,
                    Category = GlobalData.Instance.IncomeCategories.FirstOrDefault(),
                    BankIconPath = "avares://BalanceBuddyDesktop/Assets/Images/CapitalOneSavingsLogo.jpg"
                };
                TransactionService.AddIncome(income);
            }
        }
    }
}