using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalFinance.Models
{
    public class Transaction
    {
        public string Usr_OID { get; set; }
        public int ID { get; set; }
        public string TrsCode { get; set; }
        public string TrsTitle { get; set; }
        public DateTime TrsDateTime { get; set; }
        public double TrsValue { get; set; }
        public string TrsNote { get; set; }
        //[NotMapped]
        public DateTime? TrsDateTimeExp { get; set; }
        public string ExpColorLabel { get; set; }
        public string DebCredChoice { get; set; }
        public double DebCredInValue { get; set; }
    }
    public class TransactionDetailsEdit
    {
        public List<Debit> DebitsRat { get; set; }
        public List<Debit> DebitsMono { get; set; }
        public List<Credit> CreditsMono { get; set; }
        public List<string> Codes { get; set; }
    }
    public class Transactions
    {
        public IEnumerable<Transaction> Trs { get; set; }
        public List<SelectListItem> ItemListYear { get; set; }
        public List<SelectListItem> ItemListMonth { get; set; }
        public List<SelectListItem> Codes { get; set; }
    }
}
