using Microsoft.EntityFrameworkCore;
using System;

namespace PersonalFinance.Models
{
    public class Bank
    {
        public string Usr_OID { get; set; }
        public int ID { get; set; }
        public string BankName { get; set; }
        public string Iban { get; set; }
        public int BankValue { get; set; }
        public string BankNote { get; set; }
    }
}
