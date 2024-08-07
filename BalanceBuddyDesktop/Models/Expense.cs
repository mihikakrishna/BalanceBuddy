﻿using System;

namespace BalanceBuddyDesktop.Models;

public class Expense
{
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public string FormattedDate => Date.ToString("d");
    public ExpenseCategory Category { get; set; }
    public string Description { get; set; }
}