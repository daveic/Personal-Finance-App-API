﻿using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;

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
    public class ExpMonth
    {
        public string Month { get; set; }
        public Expiration ExpItem { get; set; }
    }
    public class Expirations
    {
        public IEnumerable<Expiration> ExpirationList { get; set; }
        public List<SelectListItem> ItemlistYear { get; set; }
        public List<int> UniqueMonth { get; set; }
        public List<string> UniqueMonthNames { get; set; }
        public List<ExpMonth> ExpMonth { get; set;}
       
    }
}
