﻿using System.Collections.Generic;

namespace PersonalFinance.Models
{
    public class KnownMovements_API { 
        public KnownMovement KnownMovement { get; set; }
        public IEnumerable<KnownMovement> KnownMovements { get; set; }
        public IEnumerable<Expiration> Expirations { get; set; }
    }

    
}
