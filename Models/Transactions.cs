using System;

namespace PersonalFinance.Models
{
    public class Transaction
    {
        public string Usr_OID { get; set; }
        public int ID { get; set; }
        public string TrsCode { get; set; }
        public string TrsTitle { get; set; }
        public DateTime TrsDateTime { get; set; }
        public float TrsValue { get; set; }
        public string TrsNote { get; set; }
    }
}
