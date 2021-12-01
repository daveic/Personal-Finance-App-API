using System;

namespace PersonalFinance.Models
{
    public class Transaction
    {
        public int ID { get; set; }
        public string TrsCode { get; set; }
    public string TrsTitle { get; set; }
    public DateTime TrsDateTime { get; set; }
    public int TrsValue { get; set; }
    public string TrsNote { get; set; }
    }
}
