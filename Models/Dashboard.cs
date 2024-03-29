﻿using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace PersonalFinance.Models
{
    public class DashAPIOut
    {
        public double TransactionSum { get; set; }
        public double CreditSum { get; set; }
        public double DebitSum { get; set; }
        public double TotWithDebits { get; set; }
        public double TotNoDebits { get; set; }
        public IEnumerable<Bank> Banks { get; set; }
        public Transaction Transaction { get; set; }
        public List<SelectListItem> ItemListYear { get; set; }
        public List<SelectListItem> ItemListMonth { get; set; }
        public List<SelectListItem> ItemListYearTr { get; set; }
        public List<SelectListItem> ItemListMonthTr { get; set; }
        public string Balances { get; set; }
        public List<SelectListItem> Codes { get; set; }
        public List<SelectListItem> BankList { get; set; }
        public string Transactions { get; set; }        
        public IEnumerable<Transaction> TransactionsIn { get; set; }
        public IEnumerable<Transaction> TransactionsOut { get; set; }
    }
}