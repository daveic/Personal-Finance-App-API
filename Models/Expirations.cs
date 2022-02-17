using System;

namespace PersonalFinance.Models
{
    public class Expiration
    {
        public string Usr_OID { get; set; }
        public int ID { get; set; }
        public string ExpTitle { get; set; }
        public string ExpDescription { get; set; }
        public double ExpValue { get; set; } //Diffenziato In - Out
        public DateTime ExpDateTime { get; set; } //Data scadenza
        public string ColorLabel { get; set; }
    }
}
