﻿using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;

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
    }
    public class TransactionDetailsEdit
    {
        public int ID { get; set; }
        public string User_OID { get; set; }
        public List<SelectListItem> Codes { get; set; }
    }
}
