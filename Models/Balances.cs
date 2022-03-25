using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalFinance.Models
{
    public class Balance
    {
        public string Usr_OID { get; set; }
        public int ID { get; set; }
        public DateTime BalDateTime { get; set; }
        public double ActBalance { get; set; }
        [NotMapped]
        public bool FromFU { get; set; }
    }
}