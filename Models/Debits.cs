﻿using System;

namespace PersonalFinance.Models
{
    public class Debit
    {
        public string Usr_OID { get; set; }
        public int ID { get; set; }
        public string DebCode { get; set; }
        public string DebTitle { get; set; }
        public string CredName { get; set; }
        public DateTime DebDateTime { get; set; } //Scadenza debito
        public DateTime DebInsDate { get; set; } //Data di inserimento debito
        public double RtNum { get; set; } //Numero di rate
        public double RtPaid { get; set; } //Rate pagate
        public double RemainToPay { get; set; } //Importo da pagare
        public int Multiplier { get; set; } //Frequenza rate - Ogni *n*
        public string RtFreq { get; set; } //Frequenza rate - Possibile: Settimana, Mese, Anno
        public double DebValue { get; set; }
        public string DebNote { get; set; }
        public int Exp_ID { get; set; }
        public int Hide { get; set; }
        public int FromTrs { get; set; }
    }
    public class Debit_Exp
    {
        public Debit Debit { get; set; }
        public bool FromTransaction { get; set; }
    }
}
