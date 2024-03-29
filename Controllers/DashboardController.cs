﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PersonalFinance.Models;
using PersonalFinance.Services;
using PersonalFinance.Services.EntityFramework;

//Known Movements Controller
namespace PersonalFinance.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class DashboardController : PFA_APIController
    {
        private readonly PersonalFinanceContext PersonalFinanceContext;
        private readonly IRepository repo;
        public DashboardController(IRepository repo, PersonalFinanceContext PersonalFinanceContext) : base(repo, PersonalFinanceContext)
        {
            this.repo = repo;
            this.PersonalFinanceContext = PersonalFinanceContext;
        }

        [HttpGet]
        [Route("Main")]
        public async Task<IActionResult> Dashboard_Main(string selectedYear, string selectedMonth, string selectedYearTr, string selectedMonthTr, string User_OID)
        {
            IEnumerable<Transaction> Transactions = PersonalFinanceContext.Set<Transaction>().AsNoTracking().AsQueryable().Where(x => x.Usr_OID == User_OID).ToList();
            IEnumerable<Credit> Credits = PersonalFinanceContext.Set<Credit>().AsNoTracking().AsQueryable().Where(x => x.Usr_OID == User_OID).ToList();
            IEnumerable<Debit> Debits = PersonalFinanceContext.Set<Debit>().AsNoTracking().AsQueryable().Where(x => x.Usr_OID == User_OID).Where(y => y.Hide == 0).ToList();
            IEnumerable<Bank> Banks = PersonalFinanceContext.Set<Bank>().AsNoTracking().AsQueryable().Where(x => x.Usr_OID == User_OID).ToList();
            IEnumerable<Ticket> Tickets = PersonalFinanceContext.Set<Ticket>().AsNoTracking().AsQueryable().Where(x => x.Usr_OID == User_OID).ToList();
            IEnumerable<Deposit> Deposits = PersonalFinanceContext.Set<Deposit>().AsNoTracking().AsQueryable().Where(x => x.Usr_OID == User_OID).ToList();
            IEnumerable<Balance> Balances = PersonalFinanceContext.Set<Balance>().AsNoTracking().AsQueryable().Where(x => x.Usr_OID == User_OID).ToList();
            DashAPIOut DOut = new();

            if (!Banks.Any())
            {
                Bank b = new() { Usr_OID = User_OID, BankName = "Contanti", Iban = null, ID = 0, BankValue = 0, BankNote = "Totale contanti" };
                await repo.AddBankAsync(b);
                await repo.SaveChangesAsync();
            }
            foreach (var item in Transactions)
            {
                DOut.TransactionSum += item.TrsValue;
            }
            foreach (var item in Credits)
            {
                DOut.CreditSum += item.CredValue;
            }
            foreach (var item in Debits)
            {
                DOut.DebitSum += item.DebValue;
            }
            // Totale saldo + crediti - debiti
            DOut.TotWithDebits = DOut.TransactionSum + DOut.CreditSum - DOut.DebitSum;
            // Totale saldo + crediti
            DOut.TotNoDebits = DOut.TransactionSum + DOut.CreditSum;

            //############################################################################################################################
            //FILTRI ANNO E MESE PER GRAFICO SALDO
            //############################################################################################################################
            //Trovo gli anni "unici"
            var UniqueYear = Balances.GroupBy(x => x.BalDateTime.Year)
                                        .OrderBy(x => x.Key)
                                        .Select(x => new { Year = x.Key })
                                        .ToList();
            //Creo la lista di anni "unici" per il dropdown filter del grafico saldo
            List<SelectListItem> itemlistYear = new();
            foreach (var year in UniqueYear)
            {
                SelectListItem subitem = new() { Text = year.Year.ToString(), Value = year.Year.ToString() };
                itemlistYear.Add(subitem);
            }
            //Passo alla view la lista
            DOut.ItemListYear = itemlistYear;
            //Se al caricamento della pagina ho selezionato un anno (not empty), salvo in Balances i saldi di quell'anno
            if (!String.IsNullOrEmpty(selectedYear)) Balances = Balances.AsQueryable().Where(x => x.BalDateTime.Year.ToString() == selectedYear);
            //Trovo i mesi "unici"
            var UniqueMonth = Balances.GroupBy(x => x.BalDateTime.Month)
                                        .OrderBy(x => x.Key)
                                        .Select(x => new { Month = x.Key })
                                        .ToList();
            //Creo la lista di mesi "unici" per il dropdown filter del grafico saldo
            List<SelectListItem> itemlistMonth = new();
            foreach (var month in UniqueMonth)
            {
                SelectListItem subitem = new() { Text = MonthConverter(month.Month), Value = MonthConverter(month.Month) };
                itemlistMonth.Add(subitem);
            }
            //Passo alla view la lista
            DOut.ItemListMonth = itemlistMonth;
            //Se al caricamento della pagina ho selezionato un mese (not empty), salvo in Balances i saldi di quel mese
            if (!String.IsNullOrEmpty(selectedMonth)) Balances = Balances.AsQueryable().Where(x => MonthConverter(x.BalDateTime.Month) == selectedMonth);
            //############################################################################################################################


            //############################################################################################################################
            //FILTRI ANNO E MESE PER GRAFICI TRANSAZIONI
            //############################################################################################################################
            //Trovo gli anni "unici"
            var UniqueYearTr = Transactions.GroupBy(x => x.TrsDateTime.Year)
                                            .OrderBy(x => x.Key)
                                            .Select(x => new { Year = x.Key })
                                            .ToList();
            //Creo la lista di anni "unici" per il dropdown filter del grafico saldo
            List<SelectListItem> itemlistYearTr = new();
            foreach (var year in UniqueYearTr)
            {
                SelectListItem subitem = new() { Text = year.Year.ToString(), Value = year.Year.ToString() };
                itemlistYearTr.Add(subitem);
            }
            //Passo alla view la lista
            DOut.ItemListYearTr = itemlistYearTr;
            //Se al caricamento della pagina ho selezionato un anno (not empty), salvo in Balances i saldi di quell'anno
            if (!String.IsNullOrEmpty(selectedYearTr)) Transactions = Transactions.AsQueryable().Where(x => x.TrsDateTime.Year.ToString() == selectedYearTr);
            //Trovo i mesi "unici"
            var UniqueMonthTr = Transactions.GroupBy(x => x.TrsDateTime.Month)
                                                .OrderBy(x => x.Key)
                                                .Select(x => new { Month = x.Key })
                                                .ToList();
            //Creo la lista di mesi "unici" per il dropdown filter del grafico saldo
            List<SelectListItem> itemlistMonthTr = new();
            foreach (var month in UniqueMonthTr)
            {
                SelectListItem subitem = new() { Text = MonthConverter(month.Month), Value = MonthConverter(month.Month) };
                itemlistMonthTr.Add(subitem);
            }
            //Passo alla view la lista
            DOut.ItemListMonthTr = itemlistMonthTr;
            //Se al caricamento della pagina ho selezionato un mese (not empty), salvo in Balances i saldi di quel mese
            if (!String.IsNullOrEmpty(selectedMonthTr)) Transactions = Transactions.AsQueryable().Where(x => MonthConverter(x.TrsDateTime.Month) == selectedMonthTr);
            //############################################################################################################################

            var TransactionsIn = Transactions.Where(x => x.TrsValue >= 0);
            var TransactionsOut = Transactions.Where(x => x.TrsValue < 0);

            //############################################################################################################################

            //Passo alla view la lista aggiornata e convertita in JSON
            DOut.Balances = JsonConvert.SerializeObject(Balances);

            List<SelectListItem> BankListItems = new();
            foreach (var bank in Banks)
            {
                SelectListItem bankItem = new()
                {
                    Value = bank.BankName,
                    Text = bank.BankName
                };
                BankListItems.Add(bankItem);
            }

            DOut.BankList = BankListItems;

            var UniqueCodes = Transactions.GroupBy(x => x.TrsCode)
                                           .Select(x => x.First())
                                           .ToList();
            List<SelectListItem> Codes = new();
            foreach (var item in UniqueCodes)
            {
                if (!item.TrsCode.StartsWith("CRE") && !item.TrsCode.StartsWith("DEB") && !item.TrsCode.StartsWith("MVF") && !item.TrsCode.StartsWith("SCD") && item.TrsCode != "Fast_Update" && item.TrsCode != "Nuovo ticket" && item.TrsCode != "Nuovo conto")
                {
                    SelectListItem code = new()
                    {
                        Value = item.TrsCode,
                        Text = item.TrsCode
                    };
                    Codes.Add(code);
                }                
            }
            //bool isPresent = false;
            //foreach (var credit in Credits)
            //{
            //    foreach (var item in Codes)
            //    {
            //        if (credit.CredCode == item.Value) isPresent = true;
            //    }
            //    if (isPresent is true)
            //    {
            //        SelectListItem code = new()
            //        {
            //            Value = credit.CredCode,
            //            Text = credit.CredCode
            //        };
            //        Codes.Add(code);
            //    }
            //    isPresent = false;
            //}
            //foreach (var debit in Debits)
            //{
            //    foreach (var item in Codes)
            //    {
            //        if (debit.DebCode == item.Value) isPresent = true;
            //    }
            //    if (isPresent is false)
            //    {
            //        SelectListItem code = new()
            //        {
            //            Value = debit.DebCode,
            //            Text = debit.DebCode
            //        };
            //        Codes.Add(code);
            //    }
            //    isPresent = false;
            //}
            DOut.Codes = Codes;
            DOut.Transaction = new Transaction();

            DOut.TransactionsIn = TransactionsIn;
            DOut.TransactionsOut = TransactionsOut;
            DOut.Transactions = JsonConvert.SerializeObject(Transactions);
            DOut.Banks = Banks;

            return Ok(DOut);
        }
    }
}
