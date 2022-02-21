using Microsoft.EntityFrameworkCore;
using System;

namespace PersonalFinance.Models
{
    public class Credit
    {
        public string Usr_OID { get; set; }
        public int ID { get; set; }
        public string CredCode { get; set; }
        public string CredTitle { get; set; }
        public string DebName { get; set; }
        public DateTime CredDateTime { get; set; }
        public DateTime PrevDateTime { get; set; } //Data prevista di rientro credito
        public double CredValue { get; set; }
        public string CredNote { get; set; }
    }
}
