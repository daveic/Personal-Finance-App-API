﻿using System;

namespace PersonalFinance.Models
{
    public class KnownMovement
    {
        public string Usr_OID { get; set; }
        public int ID { get; set; }
        public string KMType { get; set; }
        public string KMTitle { get; set; }
        public double KMValue { get; set; }
        public string KMNote { get; set; }
    }
}
