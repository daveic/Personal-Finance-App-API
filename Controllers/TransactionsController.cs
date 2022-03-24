using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PersonalFinance.Models;
using PersonalFinance.Services;
using PersonalFinance.Services.EntityFramework;
using PersonalFinance.Controllers;

//Transactions Controller
namespace PersonalFinance.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class TransactionsController : PFA_APIController
    {
        private readonly PersonalFinanceContext PersonalFinanceContext;
        private readonly IRepository repo;
        public TransactionsController(IRepository repo, PersonalFinanceContext PersonalFinanceContext) : base(repo, PersonalFinanceContext)
        {
            this.repo = repo;
            this.PersonalFinanceContext = PersonalFinanceContext;
        }

        [HttpGet]
        [Route("All")]
        public async Task<IActionResult> Transactions_All(string User_OID)
        {
            return Ok(await repo.GetAllTransactionsAsync(User_OID));
        }
        [HttpGet]
        [Route("Main")]
        public async Task<IActionResult> Transactions_Main(string User_OID)
        {
            IEnumerable<Transaction> Transactions = await repo.GetAllTransactionsAsync(User_OID);
            IEnumerable<Credit> Credits = await repo.GetAllCreditsAsync(User_OID);
            //edits = Credits.Where(y => y.Hide == 0);
            IEnumerable<Debit> Debits = await repo.GetAllDebitsAsync(User_OID);
            Debits = Debits.Where(y => y.Hide == 0);
            Transactions Trs = new ();
            Trs.Trs = Transactions;
            //############################################################################################################################
            //FILTRI ANNO E MESE PER GRAFICO SALDO
            //############################################################################################################################
            //Trovo gli anni "unici"
            var UniqueYear = Transactions.GroupBy(item => item.TrsDateTime.Year)
                    .Select(group => group.First())
                    .Select(item => item.TrsDateTime.Year)
                    .ToList();
            //Creo la lista di anni "unici" per il dropdown filter del grafico saldo
            List<SelectListItem> itemlistYear = new ();
            foreach (var year in UniqueYear) itemlistYear.Add(new SelectListItem() { Text = year.ToString(), Value = year.ToString() });
            //Passo alla view la lista
            Trs.ItemListYear = itemlistYear;
            //############################################################################################################################
            //Trovo i mesi "unici"
            var UniqueMonth = Transactions.GroupBy(item => item.TrsDateTime.Month)
                                .Select(group => group.First())
                                .Select(item => item.TrsDateTime.Month)
                                .ToList();
            //Creo la lista di mesi "unici" per il dropdown filter del grafico saldo
            List<SelectListItem> itemlistMonth = new ();
            foreach (var month in UniqueMonth) itemlistMonth.Add(new SelectListItem() { Text = MonthConverter(month), Value = MonthConverter(month) });
            //Passo alla view la lista
            Trs.ItemListMonth = itemlistMonth;
            //############################################################################################################################

            var UniqueCodes = Transactions.GroupBy(x => x.TrsCode)
                              .Select(x => x.First())
                              .ToList();
            List<SelectListItem> Codes = new();
            foreach (var item in UniqueCodes)
            {
                SelectListItem code = new();
                code.Value = item.TrsCode;
                code.Text = item.TrsCode;
                Codes.Add(code);
            }
            bool isPresent = false;
            foreach (var credit in Credits)
            {
                foreach (var item in Codes)
                {
                    if (credit.CredCode == item.Value) isPresent = true;
                }
                if (isPresent is false) Codes.Add(new SelectListItem() { Text = credit.CredCode, Value = credit.CredCode });
                isPresent = false;
            }
            foreach (var debit in Debits)
            {
                foreach (var item in Codes)
                {
                    if (debit.DebCode == item.Value) isPresent = true;
                }
                if (isPresent is false) Codes.Add(new SelectListItem() { Text = debit.DebCode, Value = debit.DebCode });
                isPresent = false;
            }
            Trs.Codes = Codes;
            return Ok(Trs);
        }
        [HttpGet]
        [Route("Details")]
        public async Task<IActionResult> Transaction_Details(int id, string User_OID)
        {
            return Ok(await repo.GetTransactionAsync(id, User_OID));
        }
        [HttpPost]
        [Route("Add")]
        public async Task<IActionResult> Transaction_Add([FromBody] Transaction t)
        {

            await Transaction_Credit_Debit_UpdateAsync(t);
            this.PersonalFinanceContext.Add(t);
            await repo.SaveChangesAsync();
            return Ok();
        }
        

        [ApiExplorerSettings(IgnoreApi = true)]
        [NonAction]
        public async Task<int> Transaction_Credit_Debit_UpdateAsync(Transaction t)
        {
            var Credits = PersonalFinanceContext.Set<Credit>().AsNoTracking().AsQueryable().Where(x => x.Usr_OID == t.Usr_OID).ToList();
            var Debits = PersonalFinanceContext.Set<Debit>().AsNoTracking().AsQueryable().Where(x => x.Usr_OID == t.Usr_OID).Where(y => y.Hide == 0).ToList();


            if (t.DebCredInValue == 0 && t.DebCredChoice.StartsWith("DEB"))
            {
                foreach (var debit in Debits)
                {
                    if (t.DebCredChoice == debit.DebCode)
                    {
                        debit.RemainToPay -= debit.DebValue / debit.RtNum;
                        debit.RtPaid += 1;
                        var exp = PersonalFinanceContext.Set<Expiration>().AsNoTracking().AsQueryable().Where(x => x.Usr_OID == debit.Usr_OID).FirstOrDefault(x => x.ID == (debit.Exp_ID + Convert.ToInt32(debit.RtPaid - 1)));
                        this.PersonalFinanceContext.Remove(exp);
                        _ = PersonalFinanceContext.SaveChanges() > 0;

                        if (debit.RemainToPay <= 0)
                        {
                            debit.Hide = 1;
                            PersonalFinanceContext.Attach(debit);
                            PersonalFinanceContext.Entry(debit).State =
                                Microsoft.EntityFrameworkCore.EntityState.Modified;
                            _ = PersonalFinanceContext.SaveChanges() > 0;
                            //await repo.DeleteDebitAsync(debit);
                        }
                        else
                        {
                            PersonalFinanceContext.Attach(debit);
                            PersonalFinanceContext.Entry(debit).State =
                                Microsoft.EntityFrameworkCore.EntityState.Modified;
                            _ = PersonalFinanceContext.SaveChanges() > 0;
                        }
                        t.TrsTitle = "Pagamento " + debit.RtPaid + "° rata ";
                        t.TrsCode = debit.DebCode;
                        t.TrsDateTime = DateTime.UtcNow;
                        t.TrsValue = debit.DebValue / debit.RtNum;
                        t.TrsNote = t.TrsTitle + " - " + t.TrsCode;
                    }
                }


            }
            if (t.DebCredInValue != 0)
            {
                if (t.DebCredChoice.StartsWith("DEB"))
                {
                    foreach (var debit in Debits)
                    {
                        if (t.DebCredChoice == debit.DebCode)
                        {
                            debit.RemainToPay -= t.DebCredInValue;
                            var exp = PersonalFinanceContext.Set<Expiration>().AsNoTracking().AsQueryable().Where(x => x.Usr_OID == debit.Usr_OID).FirstOrDefault(x => x.ID == debit.Exp_ID);
                            this.PersonalFinanceContext.Remove(exp);
                            _ = PersonalFinanceContext.SaveChanges() > 0;

                            if (debit.RemainToPay <= 0)
                            {

                                await repo.DeleteDebitAsync(debit);
                            }
                            else
                            {
                                Expiration newexp = new()
                                {
                                    Usr_OID = debit.Usr_OID,
                                    ExpTitle = debit.DebCode,
                                    ExpDescription = "Scadenza - " + debit.DebTitle,
                                    ExpDateTime = debit.DebDateTime,
                                    ColorLabel = "red",
                                    ExpValue = debit.RemainToPay
                                };
                                await repo.AddExpirationAsync(newexp);
                                PersonalFinanceContext.Attach(debit);
                                PersonalFinanceContext.Entry(debit).State =
                                    Microsoft.EntityFrameworkCore.EntityState.Modified;
                                _ = PersonalFinanceContext.SaveChanges() > 0;
                            }
                            t.TrsTitle = "Pagamento debito";
                            t.TrsCode = debit.DebCode;
                            t.TrsDateTime = DateTime.UtcNow;
                            t.TrsValue = -t.DebCredInValue;
                            t.TrsNote = t.TrsTitle + " - " + t.TrsCode;
                        }
                    }
                }
                else if (t.DebCredChoice.StartsWith("CRE"))
                {
                    foreach (var credit in Credits)
                    {
                        if (t.DebCredChoice == credit.CredCode)
                        {
                            var expCred = PersonalFinanceContext.Set<Expiration>().AsNoTracking().AsQueryable().Where(x => x.Usr_OID == credit.Usr_OID).FirstOrDefault(x => x.ID == credit.Exp_ID);
                            this.PersonalFinanceContext.Remove(expCred);
                            _ = PersonalFinanceContext.SaveChanges() > 0;
                            credit.CredValue -= t.DebCredInValue;
                            if (credit.CredValue <= 0)
                            {
                                await repo.DeleteCreditAsync(credit);
                            }
                            else
                            {
                                Expiration exp = new()
                                {
                                    Usr_OID = credit.Usr_OID,
                                    ExpTitle = credit.CredCode,
                                    ExpDescription = "Rientro previsto - " + credit.CredTitle,
                                    ExpDateTime = credit.PrevDateTime,
                                    ColorLabel = "green",
                                    ExpValue = credit.CredValue
                                };
                                await repo.AddExpirationAsync(exp);
                                _ = PersonalFinanceContext.SaveChanges() > 0;
                                credit.Exp_ID = PersonalFinanceContext.Set<Expiration>().AsNoTracking().AsQueryable().Where(x => x.Usr_OID == credit.Usr_OID).OrderBy(x => x.ID).Last().ID;
                                PersonalFinanceContext.Attach(credit);
                                PersonalFinanceContext.Entry(credit).State =
                                    Microsoft.EntityFrameworkCore.EntityState.Modified;
                                _ = PersonalFinanceContext.SaveChanges() > 0;
                            }
                            t.TrsTitle = "Rientro credito";
                            t.TrsCode = credit.CredCode;
                            t.TrsDateTime = DateTime.UtcNow;
                            t.TrsValue = t.DebCredInValue;
                            t.TrsNote = t.TrsTitle + " - " + t.TrsCode;
                        }
                    }
                }
            }

            if (t.DebCredInValue == 0 && t.DebCredChoice is not null)
            {
                if (t.DebCredChoice == "NCred")
                {
                    Credit model = new()
                    {
                        Usr_OID = t.Usr_OID,
                        CredCode = "CRE " + t.TrsTitle,
                        CredDateTime = DateTime.Now,
                        CredValue = -t.TrsValue,
                        CredTitle = t.TrsTitle,
                        CredNote = t.TrsNote,
                        PrevDateTime = (DateTime)t.TrsDateTimeExp
                    };
                    await Credit_Add_Service(model);
                    t.TrsCode = model.CredCode;
                } else if (t.DebCredChoice == "NDeb")
                {
                    Debit model = new();
                    model.Usr_OID = t.Usr_OID;
                    model.DebCode = "DEB " + t.TrsTitle;
                    model.DebInsDate = DateTime.Now;
                    model.DebValue = t.TrsValue;
                    model.DebTitle = t.TrsTitle;
                    model.DebNote = t.TrsNote;
                    model.RemainToPay = t.TrsValue;
                    model.RtPaid = 0;
                    model.RtNum = 1;
                    model.Multiplier = 0;
                    model.DebDateTime = (DateTime)t.TrsDateTimeExp;
                    await Debit_Add_Service(model);
                    t.TrsCode = model.DebCode;
                }
            }

            //    if (t.TrsValue < 0)
            //{
            //    foreach (var debit in Debits)
            //    {
            //        if (t.TrsCode == debit.DebCode)
            //        {
            //            debit.RemainToPay += t.TrsValue;
            //            debit.RtPaid += (-t.TrsValue) / (debit.DebValue / debit.RtNum);                        
            //            var exp = PersonalFinanceContext.Set<Expiration>().AsNoTracking().AsQueryable().Where(x => x.Usr_OID == debit.Usr_OID).FirstOrDefault(x => x.ID == (debit.Exp_ID + Convert.ToInt32(debit.RtPaid - 1)));
            //            this.PersonalFinanceContext.Remove(exp);
            //            _ = PersonalFinanceContext.SaveChanges() > 0;
            //            if (debit.RemainToPay <= 0)
            //            {
            //                await repo.DeleteDebitAsync(debit);
            //            }
            //            else
            //            {
            //                PersonalFinanceContext.Attach(debit);
            //                PersonalFinanceContext.Entry(debit).State =
            //                    Microsoft.EntityFrameworkCore.EntityState.Modified;
            //                _ = PersonalFinanceContext.SaveChanges() > 0;
            //            }
            //        }
            //    }
            //    if (t.TrsCode.StartsWith("CRE"))
            //    {
            //        Credit model = new()
            //        {
            //            Usr_OID = t.Usr_OID,
            //            CredCode = t.TrsCode,
            //            CredDateTime = DateTime.UtcNow,
            //            CredValue = -t.TrsValue,
            //            CredTitle = "Prestito/Anticipo",
            //            CredNote = "",
            //            PrevDateTime = (DateTime)t.TrsDateTimeExp
            //        };
            //        await Credit_Add_Service(model);
            //    }
            //}
            //if (t.TrsValue > 0)
            //{
            //    foreach (var credit in Credits)
            //    {
            //        if (t.TrsCode == credit.CredCode)
            //        {
            //            var expCred = PersonalFinanceContext.Set<Expiration>().AsNoTracking().AsQueryable().Where(x => x.Usr_OID == credit.Usr_OID).FirstOrDefault(x => x.ID == credit.Exp_ID);
            //            this.PersonalFinanceContext.Remove(expCred);
            //            _ = PersonalFinanceContext.SaveChanges() > 0;
            //            credit.CredValue -= t.TrsValue;
            //            if (credit.CredValue <= 0)
            //            {
            //                await repo.DeleteCreditAsync(credit);                             
            //            }
            //            else
            //            {
            //                Expiration e = new()
            //                {
            //                    Usr_OID = credit.Usr_OID,
            //                    ExpTitle = credit.CredTitle,
            //                    ExpDescription = "Rientro previsto - " + credit.CredTitle,
            //                    ExpDateTime = credit.PrevDateTime,
            //                    ColorLabel = "green",
            //                    ExpValue = credit.CredValue
            //                };
            //                this.PersonalFinanceContext.Add(e);
            //                credit.Exp_ID = PersonalFinanceContext.Set<Expiration>().AsNoTracking().AsQueryable().Where(x => x.Usr_OID == credit.Usr_OID).OrderBy(x => x.ID).Last().ID;//
            //                _ = PersonalFinanceContext.SaveChanges() > 0;
            //                PersonalFinanceContext.Attach(credit);
            //                PersonalFinanceContext.Entry(credit).State =
            //                    Microsoft.EntityFrameworkCore.EntityState.Modified;
            //            }
            //        }
            //    }
            //    _ = PersonalFinanceContext.SaveChanges() > 0;
            //    if (t.TrsCode.StartsWith("DEB"))
            //    {
            //        Debit model = new();
            //        model.Usr_OID = t.Usr_OID;
            //        model.DebCode = t.TrsCode;
            //        model.DebInsDate = DateTime.UtcNow;
            //        model.DebValue = t.TrsValue;
            //        model.DebTitle = "Prestito/Anticipo";
            //        model.DebNote = "";
            //        model.RemainToPay = t.TrsValue;
            //        model.RtPaid = 0;
            //        model.RtNum = 1;
            //        model.Multiplier = 0;
            //        model.DebDateTime = (DateTime)t.TrsDateTimeExp;
            //        await Debit_Add_Service(model);
            //    }
            //}
            return 1;
        }
        //[ApiExplorerSettings(IgnoreApi = true)]
        //[NonAction]
        //public async Task<int> Transaction_Credit_Debit_RestoreAsync(Transaction t)
        //{

        //    return 1;
     
        //}

        [HttpGet]
        [Route("DetailsEdit")]
        public virtual Task<TransactionDetailsEdit> Transaction_Details_Edit(string User_OID)
        {

            var UniqueCodes = PersonalFinanceContext.Set<Transaction>().AsNoTracking().AsQueryable()
                .Where(x => x.Usr_OID == User_OID)
                .GroupBy(x => x.TrsCode)
                .Select(x => x.First())
                .ToList();

            var Credits = PersonalFinanceContext.Set<Credit>().AsNoTracking().AsQueryable().Where(x => x.Usr_OID == User_OID).ToList();
            var Debits = PersonalFinanceContext.Set<Debit>().AsNoTracking().AsQueryable().Where(x => x.Usr_OID == User_OID).Where(y => y.Hide == 0).ToList();

            TransactionDetailsEdit APIData = new();

            APIData.DebitsRat = Debits.Where(x => x.RtNum > 1).ToList();
            APIData.DebitsMono = Debits.Where(x => x.RtNum == 1).ToList();
            APIData.CreditsMono = Credits;
            APIData.Codes = new();
            foreach (var item in UniqueCodes)
            {
                APIData.Codes.Add(item.TrsCode);
            }
            bool isPresent = false;
            foreach (var credit in Credits)
            {
                foreach (var item in APIData.Codes)
                {
                    if (credit.CredCode == item) isPresent = true;
                }
                if (isPresent is true)
                {
                    APIData.Codes.Add(credit.CredCode);
                }
                isPresent = false;
            }
            foreach (var debit in Debits)
            {
                foreach (var item in APIData.Codes)
                {
                    if (debit.DebCode == item) isPresent = true;
                }
                if (isPresent is false)
                {
                    APIData.Codes.Add(debit.DebCode);
                }
                isPresent = false;
            }
            return Task.FromResult(APIData);
        }
        [HttpPut]
        [Route("Update")]
        public async Task<IActionResult> Transaction_Edit(Transaction t)
        {
            Transaction trsOld = PersonalFinanceContext.Set<Transaction>().AsNoTracking().AsQueryable().Where(x => x.Usr_OID == t.Usr_OID).FirstOrDefault(x => x.ID == t.ID);
            if(t.TrsCode == "CRE" )
            {
                Credit c = new()
                {
                    Usr_OID = t.Usr_OID,
                    CredCode = t.TrsCode,
                    CredDateTime = t.TrsDateTime,
                    CredValue = -t.TrsValue,
                    CredTitle = "Prestito/Anticipo",
                    CredNote = t.TrsNote,
                    PrevDateTime = (DateTime)t.TrsDateTimeExp
                };
                await Credit_Edit_Service(c);
            } else if (t.TrsCode == "DEB")
            {
              
                Debit d = new()
                {
                    Usr_OID = t.Usr_OID,
                    DebCode = t.TrsCode,
                    DebInsDate = DateTime.UtcNow,
                    DebValue = t.TrsValue,
                    DebTitle = "Prestito/Anticipo",
                    DebNote = t.TrsNote,
                    RemainToPay = t.TrsValue,
                    RtPaid = 0,
                    RtNum = 1,
                    Multiplier = 0,
                    DebDateTime = (DateTime)t.TrsDateTimeExp
                };
                await Debit_Edit_Service(d);
            }

            await repo.UpdateTransactionAsync(t);
            await repo.SaveChangesAsync();
            return Ok(t);
        }
        [HttpDelete]
        [Route("Delete")]
        public async Task<IActionResult> Transaction_Delete(int id, string User_OID)
        {
            var t = await repo.GetTransactionAsync(id, User_OID);
            if (t.DebCredInValue == 0 && t.DebCredChoice.StartsWith("DEB"))
            {
                var Debits = PersonalFinanceContext.Set<Debit>().AsNoTracking().AsQueryable().Where(x => x.Usr_OID == t.Usr_OID).Where(y => y.Hide == 0).ToList();
                
                foreach (var debit in Debits)
                {
                    if (t.DebCredChoice == debit.DebCode)
                    {
                        var exp = PersonalFinanceContext.Set<Expiration>().AsNoTracking().AsQueryable().Where(x => x.Usr_OID == t.Usr_OID).FirstOrDefault(x => x.ID == (debit.Exp_ID + Convert.ToInt32(debit.RtPaid - 1)+1));
                        debit.RemainToPay += debit.DebValue / debit.RtNum;
                        debit.RtPaid -= 1;
                        if(debit.RtFreq == "Mesi") debit.DebInsDate = exp.ExpDateTime.AddMonths(-debit.Multiplier);
                        if (debit.RtFreq == "Anni") debit.DebInsDate = exp.ExpDateTime.AddMonths(-debit.Multiplier);
                        await Debit_Edit_Service(debit);
                    }
                    else
                    {
                        var DebitsHide = PersonalFinanceContext.Set<Debit>().AsNoTracking().AsQueryable().Where(x => x.Usr_OID == t.Usr_OID).Where(y => y.Hide == 1).ToList();
                        foreach (var item in DebitsHide)
                        {
                            if( t.DebCredChoice == item.DebCode)
                            {
                                item.Hide = 0;
                                await Debit_Add_Service(item);
                            }
                        }
                       
                        //var trs = PersonalFinanceContext.Set<Transaction>().AsNoTracking().AsQueryable().Where(x => x.Usr_OID == t.Usr_OID).Where(x => x.TrsCode == t.TrsCode);
                        //var trscount = trs.Count();
                        //var trsfrq = trs.OrderByDescending(x => x.TrsDateTime).Take(2);
                        //int monthnum = Math.Abs(12 * (trsfrq.Last().TrsDateTime.Year - trsfrq.First().TrsDateTime.Year) + trsfrq.Last().TrsDateTime.Month - trsfrq.First().TrsDateTime.Month);
                        //Debit model = new();
                        //model.Usr_OID = t.Usr_OID;
                        //model.DebCode = t.TrsCode;
                        //model.DebInsDate = DateTime.Now;
                        //model.DebValue = t.TrsValue;
                        //model.DebTitle = t.TrsTitle;
                        //model.DebNote = "Ultima rata del debito con codice '" + t.TrsCode + "' ricreata a seguito di eliminazione o modifica di una transazione.";
                        //model.RemainToPay = t.TrsValue;
                        //model.RtPaid = trscount;
                        //model.RtNum = trscount + 1;
                        //model.Multiplier = monthnum;
                        //model.DebDateTime = (DateTime)trsfrq.First().TrsDateTime.AddMonths(monthnum);
                        //await Debit_Add_Service(model);                       
                    }
                    //else se non lo trova significa che è stato rimosso del tutto, errore - l'utente se lo ricrea
                }
            }
            if (t.DebCredInValue != 0 && t.DebCredChoice.StartsWith("DEB"))
            {
                var Debits = PersonalFinanceContext.Set<Debit>().AsNoTracking().AsQueryable().Where(x => x.Usr_OID == t.Usr_OID).ToList();
                foreach (var debit in Debits)
                {
                    if (t.DebCredChoice == debit.DebCode)
                    {
                        debit.RemainToPay += t.DebCredInValue;
                        await Debit_Edit_Service(debit);
                    }
                    else
                    {
                        //Debit model = new();
                        //model.Usr_OID = t.Usr_OID;
                        //model.DebCode = t.TrsCode;
                        //model.DebInsDate = DateTime.Now;
                        //model.DebValue = t.TrsValue;
                        //model.DebTitle = t.TrsTitle;
                        //model.DebNote = "Ultima rata del debito con codice '" + t.TrsCode + "' ricreata a seguito di eliminazione o modifica di una transazione.";
                        //model.RemainToPay = t.TrsValue;
                        //model.RtPaid = trscount;
                        //model.RtNum = trscount + 1;
                        //model.Multiplier = monthnum;
                        //model.DebDateTime = (DateTime)trsfrq.First().TrsDateTime.AddMonths(monthnum);
                        //await Debit_Add_Service(model);
                    }
                }
            }
            if (t.DebCredInValue != 0 && t.DebCredChoice.StartsWith("CRE"))
            {
                var Credits = PersonalFinanceContext.Set<Credit>().AsNoTracking().AsQueryable().Where(x => x.Usr_OID == t.Usr_OID).ToList();
                foreach (var credit in Credits)
                {
                    if(t.DebCredChoice == credit.CredCode)
                    {
                        credit.CredValue += t.DebCredInValue;
                        await Credit_Edit_Service(credit);
                    }

        
                }
            }
            //se erano crediti o debiti e sono stati rimossi coon la trans, li ricreo:
            /*if (t.DebCredChoice == "NCred")
                {
                    Credit model = new()
                    {
                        Usr_OID = t.Usr_OID,
                        CredCode = "CRE " + t.TrsTitle,
                        CredDateTime = DateTime.Now,
                        CredValue = -t.TrsValue,
                        CredTitle = t.TrsTitle,
                        CredNote = t.TrsNote,
                        PrevDateTime = (DateTime)t.TrsDateTimeExp
                    };
                    await Credit_Add_Service(model);
                    t.TrsCode = model.CredCode;
                } else if (t.DebCredChoice == "NDeb")
                {
                    Debit model = new();
                    model.Usr_OID = t.Usr_OID;
                    model.DebCode = "DEB " + t.TrsTitle;
                    model.DebInsDate = DateTime.Now;
                    model.DebValue = t.TrsValue;
                    model.DebTitle = t.TrsTitle;
                    model.DebNote = t.TrsNote;
                    model.RemainToPay = t.TrsValue;
                    model.RtPaid = 0;
                    model.RtNum = 1;
                    model.Multiplier = 0;
                    model.DebDateTime = (DateTime)t.TrsDateTimeExp;
                    await Debit_Add_Service(model);
                    t.TrsCode = model.DebCode;
                }*/
            await repo.DeleteTransactionAsync(t);
            await repo.SaveChangesAsync();
            return Ok(t);
        }


    }
}
