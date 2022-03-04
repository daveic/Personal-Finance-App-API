﻿using System.Collections.Generic;

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
        public int Exp_ID { get; set; }
        public string input_value { get; set; }
    }

    public class KnownMovement_Exp : KnownMovement
    {
        public bool On_Exp { get; set; }
    }

    public class KnownMovements_API
    {
        public KnownMovement KnownMovement { get; set; }
        public IEnumerable<KnownMovement> KnownMovements { get; set; }
        public IEnumerable<Expiration> Expirations { get; set; }
    }
}
