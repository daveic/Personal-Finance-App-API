﻿using Microsoft.EntityFrameworkCore;
using System;

namespace PersonalFinance.Models
{
    public class Deposit
    {
        public string Usr_OID { get; set; }
        public int ID { get; set; }
        public string BankName { get; set; }
        public DateTime InitDate { get; set; }
        public DateTime EndDate { get; set; }
        public double DepValue { get; set; }
        public double PercGrow { get; set; }
        public string BankNote { get; set; }
    }
}
