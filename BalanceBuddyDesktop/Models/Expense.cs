﻿using System;

namespace BalanceBuddyDesktop.Models;

public class Expense
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public ExpenseCategory Category { get; set; }
    public string Description { get; set; }
}