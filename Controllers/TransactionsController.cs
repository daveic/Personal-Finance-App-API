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
            Transactions Trs =   new()
            {
                Trs = Transactions
            };
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
                if (!item.TrsCode.StartsWith("CRE") && !item.TrsCode.StartsWith("DEB") && !item.TrsCode.StartsWith("MVF") && !item.TrsCode.StartsWith("SCD") && item.TrsCode != "Fast_Update")
                {
                    SelectListItem code = new();
                    code.Value = item.TrsCode;
                    code.Text = item.TrsCode;
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
            //    if (isPresent is false) Codes.Add(new SelectListItem() { Text = credit.CredCode, Value = credit.CredCode });
            //    isPresent = false;
            //}
            //foreach (var debit in Debits)
            //{
            //    foreach (var item in Codes)
            //    {
            //        if (debit.DebCode == item.Value) isPresent = true;
            //    }
            //    if (isPresent is false) Codes.Add(new SelectListItem() { Text = debit.DebCode, Value = debit.DebCode });
            //    isPresent = false;
            //}
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
            var KnownMovements = PersonalFinanceContext.Set<KnownMovement>().AsNoTracking().AsQueryable().Where(x => x.Usr_OID == t.Usr_OID).ToList();
            var Expirations = PersonalFinanceContext.Set<Expiration>().AsNoTracking().AsQueryable().Where(x => x.Usr_OID == t.Usr_OID);
            Expirations = Expirations.Where(x => x.ColorLabel.StartsWith("rgb"));

            if (t.DebCredInValue == 0 && t.DebCredChoice.StartsWith("DEB"))
            {
                foreach (var debit in Debits)
                {
                    if (t.DebCredChoice == debit.DebCode)
                    {
                        if (debit.RtNum - debit.RtPaid == 1)
                        {
                            debit.Hide = 1;
                            var exp = PersonalFinanceContext.Set<Expiration>().AsNoTracking().AsQueryable().Where(x => x.Usr_OID == debit.Usr_OID).FirstOrDefault(x => x.ID == (debit.Exp_ID + Convert.ToInt32(debit.RtPaid)));
                            t.TrsDateTimeExp = exp.ExpDateTime;
                            t.ExpColorLabel = "red";
                            this.PersonalFinanceContext.Remove(exp);
                            PersonalFinanceContext.Attach(debit);
                            PersonalFinanceContext.Entry(debit).State =
                                Microsoft.EntityFrameworkCore.EntityState.Modified;
                            _ = PersonalFinanceContext.SaveChanges() > 0;
                            t.TrsTitle = "Pagamento " + (debit.RtPaid + 1) + "° rata ";
                        }
                        else
                        {
                            debit.RemainToPay -= debit.DebValue / debit.RtNum;
                            debit.RtPaid += 1;
                            var exp = PersonalFinanceContext.Set<Expiration>().AsNoTracking().AsQueryable().Where(x => x.Usr_OID == debit.Usr_OID).FirstOrDefault(x => x.ID == (debit.Exp_ID + Convert.ToInt32(debit.RtPaid - 1)));
                            t.TrsDateTimeExp = exp.ExpDateTime;
                            t.ExpColorLabel = "red";
                            this.PersonalFinanceContext.Remove(exp);
                            _ = PersonalFinanceContext.SaveChanges() > 0;
                            PersonalFinanceContext.Attach(debit);
                            PersonalFinanceContext.Entry(debit).State =
                                Microsoft.EntityFrameworkCore.EntityState.Modified;
                            _ = PersonalFinanceContext.SaveChanges() > 0;
                            t.TrsTitle = "Pagamento " + debit.RtPaid + "° rata ";
                        }
                        t.TrsCode = debit.DebCode;
                        t.TrsDateTime = DateTime.UtcNow;
                        t.TrsValue = - debit.DebValue / debit.RtNum;//
                        t.TrsNote = t.TrsTitle + " - " + t.TrsCode;
                    }
                }
            }
            if (t.DebCredInValue != 0)
            {
                if (t.DebCredChoice.StartsWith("DEB"))
                {
                    foreach (var d in Debits)
                    {
                        if (t.DebCredChoice == d.DebCode)
                        {
                            double ToPay = d.RemainToPay - t.DebCredInValue;
                            var exp = PersonalFinanceContext.Set<Expiration>().AsNoTracking().AsQueryable().Where(x => x.Usr_OID == d.Usr_OID).FirstOrDefault(x => x.ID == d.Exp_ID);
                            t.TrsDateTimeExp = exp.ExpDateTime;
                            t.ExpColorLabel = "red";
                            this.PersonalFinanceContext.Remove(exp);
                            _ = PersonalFinanceContext.SaveChanges() > 0;

                            if (ToPay <= 0)
                            {
                                d.Hide = 1;
                                PersonalFinanceContext.Attach(d);
                                PersonalFinanceContext.Entry(d).State =
                                    Microsoft.EntityFrameworkCore.EntityState.Modified;
                                _ = PersonalFinanceContext.SaveChanges() > 0;
                            }
                            else
                            {
                                d.RemainToPay -= t.DebCredInValue;
                                Expiration newexp = new()
                                {
                                    Usr_OID = d.Usr_OID,
                                    ExpTitle = d.DebCode,
                                    ExpDescription = "Scadenza - " + d.DebTitle,
                                    ExpDateTime = d.DebDateTime,
                                    ColorLabel = "red",
                                    ExpValue = d.RemainToPay
                                };
                                //await repo.AddExpirationAsync(newexp);
                                this.PersonalFinanceContext.Add(newexp);
                                _ = PersonalFinanceContext.SaveChanges() > 0;
                                d.Exp_ID = PersonalFinanceContext.Set<Expiration>().AsNoTracking().AsQueryable().Where(x => x.Usr_OID == d.Usr_OID).OrderBy(x => x.ID).Last().ID;
                                PersonalFinanceContext.Attach(d);
                                PersonalFinanceContext.Entry(d).State =
                                    Microsoft.EntityFrameworkCore.EntityState.Modified;
                                _ = PersonalFinanceContext.SaveChanges() > 0;
                            }
                            t.TrsTitle = "Pagamento debito";
                            t.TrsCode = d.DebCode;
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
                            t.TrsDateTimeExp = expCred.ExpDateTime;
                            t.ExpColorLabel = "green";
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
                        DebName = t.DCName,
                        CredTitle = t.TrsTitle,
                        CredNote = t.TrsNote,
                        PrevDateTime = (DateTime)t.TrsDateTimeExp,
                        FromTrs = 1
                    };
                    await Credit_Add_Service(model);
                    t.TrsCode = model.CredCode;
                } else if (t.DebCredChoice == "NDeb")
                {
                    Debit model = new()
                    {
                        Usr_OID = t.Usr_OID,
                        DebCode = "DEB " + t.TrsTitle,
                        DebInsDate = DateTime.Now,
                        DebValue = t.TrsValue,
                        CredName = t.DCName,
                        DebTitle = t.TrsTitle,
                        DebNote = t.TrsNote,
                        RemainToPay = t.TrsValue,
                        RtPaid = 0,
                        RtNum = 1,
                        Multiplier = 0,
                        DebDateTime = (DateTime)t.TrsDateTimeExp,
                        FromTrs = 1
                    };
                    await Debit_Add_Service(model);
                    t.TrsCode = model.DebCode;
                }
                else if (t.DebCredChoice.StartsWith("MVF"))
                {
                    foreach (var km in KnownMovements)
                    {
                        if (t.DebCredChoice == km.KMTitle)
                        {
                            var expMvf = PersonalFinanceContext.Set<Expiration>().AsNoTracking().AsQueryable().Where(x => x.Usr_OID == km.Usr_OID).FirstOrDefault(x => x.ID == km.Exp_ID);
                            t.TrsDateTimeExp = expMvf.ExpDateTime;
                            t.ExpColorLabel = "orange";
                            this.PersonalFinanceContext.Remove(expMvf);
                            _ = PersonalFinanceContext.SaveChanges() > 0;

                            t.TrsTitle = "Movimento fisso";
                            t.TrsCode = km.KMTitle;
                            t.TrsDateTime = DateTime.UtcNow;
                            t.TrsValue = km.KMValue;
                            t.TrsNote = "Movimento fisso - " + t.TrsCode;
                        }
                    }
                }
                else if (t.DebCredChoice.StartsWith("SCD"))
                {
                    Expiration ExpirationToDelete = new();
                    foreach (var exp in Expirations)
                    {
                        if (exp.ExpDateTime.Month == DateTime.Today.Month)
                        {
                            if (t.DebCredChoice == exp.ExpTitle)
                            {
                                t.TrsDateTimeExp = exp.ExpDateTime;
                                t.ExpColorLabel = exp.ColorLabel;
                                t.TrsTitle = "Scadenza";
                                t.TrsCode = exp.ExpTitle;
                                t.TrsDateTime = DateTime.UtcNow;
                                t.TrsValue = exp.ExpValue;
                                t.TrsNote = t.TrsTitle + " - " + t.TrsCode;
                                ExpirationToDelete = exp;
                            }
                        }
                    }
                    this.PersonalFinanceContext.Remove(ExpirationToDelete);
                    _ = PersonalFinanceContext.SaveChanges() > 0;
                }
            }
            return 1;
        }


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
            var KnownMovements = PersonalFinanceContext.Set<KnownMovement>().AsNoTracking().AsQueryable().Where(x => x.Usr_OID == User_OID).ToList();
            var Expirations = PersonalFinanceContext.Set<Expiration>().AsNoTracking().AsQueryable().Where(x => x.Usr_OID == User_OID);
            var ExpirationList = Expirations.Where(x => x.ColorLabel.StartsWith("rgb")).ToList();

            List<Expiration> ExpToShow = new();
            List<Expiration> ExpToShowOnExp = new();
            List<Expiration> ExpToShowAll = new();
            TransactionDetailsEdit APIData = new();


            APIData.DebitsRat = Debits.Where(x => x.RtNum > 1).ToList();
            APIData.DebitsMono = Debits.Where(x => x.RtNum == 1).ToList();
            APIData.CreditsMono = Credits;
            foreach (var exp in ExpirationList)
            {
                if(exp.ExpDateTime.Month == DateTime.Today.Month)
                {
                    ExpToShow.Add(new Expiration() { ExpTitle = exp.ExpTitle, ExpValue = exp.ExpValue, ColorLabel = exp.ColorLabel, ExpDescription = exp.ExpDescription });
                }                
            }
            

            foreach (var km in KnownMovements)
            {
                if(km.On_Exp is true) ExpToShowOnExp.Add(new Expiration() { ExpTitle = km.KMTitle, ExpValue = km.KMValue, ColorLabel = "orange" });
                ExpToShowAll.Add(new Expiration() { ExpTitle = km.KMTitle, ExpValue = km.KMValue, ColorLabel = "orange" });                
            }
            APIData.MonthExpirations = ExpToShow.Concat(ExpToShowAll).ToList();
            APIData.MonthExpirationsOnExp = ExpToShow.Concat(ExpToShowOnExp).ToList();
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
        //[HttpPut]
        //[Route("Update")]
        //public async Task<IActionResult> Transaction_Edit(Transaction t)
        //{
        //    Transaction trsOld = PersonalFinanceContext.Set<Transaction>().AsNoTracking().AsQueryable().Where(x => x.Usr_OID == t.Usr_OID).FirstOrDefault(x => x.ID == t.ID);
        //    await Transaction_Delete(trsOld.ID, trsOld.Usr_OID);
        //    await Transaction_Add(t);
            
        //    return Ok(t);
        //}
        [HttpDelete]
        [Route("Delete")]
        public async Task<IActionResult> Transaction_Delete(int id, string User_OID)
        {
            var t = await repo.GetTransactionAsync(id, User_OID);
            if (t.DebCredInValue == 0 && t.DebCredChoice.StartsWith("DEB"))
            {
                var Debits = PersonalFinanceContext.Set<Debit>().AsNoTracking().AsQueryable().Where(x => x.Usr_OID == t.Usr_OID).ToList();
                
                foreach (var debit in Debits)
                {
                    if (t.DebCredChoice == debit.DebCode && debit.Hide == 0)
                    {
                        var exp = PersonalFinanceContext.Set<Expiration>().AsNoTracking().AsQueryable().Where(x => x.Usr_OID == t.Usr_OID).FirstOrDefault(x => x.ID == (debit.Exp_ID + Convert.ToInt32(debit.RtPaid - 1)+1));
                        debit.RemainToPay += debit.DebValue / debit.RtNum;
                        debit.RtPaid -= 1;
                        if(debit.RtFreq == "Mesi") debit.DebInsDate = exp.ExpDateTime.AddMonths(-debit.Multiplier);
                        if (debit.RtFreq == "Anni") debit.DebInsDate = exp.ExpDateTime.AddYears(-debit.Multiplier);
                        await Debit_Edit_Service(debit);
                    } else if (t.DebCredChoice == debit.DebCode && debit.Hide == 1)
                    {
                        debit.Hide = 0;
                        if (debit.RtFreq == "Mesi") debit.DebInsDate = debit.DebDateTime.AddMonths(-debit.Multiplier);
                        if (debit.RtFreq == "Anni") debit.DebInsDate = debit.DebDateTime.AddYears(-debit.Multiplier);
                        await Debit_Edit_Service(debit);                    
                    }
                }
            }
            if (t.DebCredInValue != 0 && t.DebCredChoice.StartsWith("DEB"))
            {
                var Debits = PersonalFinanceContext.Set<Debit>().AsNoTracking().AsQueryable().Where(x => x.Usr_OID == t.Usr_OID).ToList();

                foreach (var debit in Debits)
                {
                    if (t.DebCredChoice == debit.DebCode && debit.Hide == 0)
                    {
                        debit.RemainToPay += t.DebCredInValue;
                        await Debit_Edit_Service(debit);
                    } else if (t.DebCredChoice == debit.DebCode && debit.Hide == 1)
                    {
                        debit.Hide = 0;
                        await Debit_Edit_Service(debit);
                    }
                }
            }
            if (t.DebCredInValue != 0 && t.DebCredChoice.StartsWith("CRE"))
            {
                var Credits = PersonalFinanceContext.Set<Credit>().AsNoTracking().AsQueryable().Where(x => x.Usr_OID == t.Usr_OID).ToList();

                foreach (var credit in Credits)
                {
                    if(t.DebCredChoice == credit.CredCode && credit.Hide == 0)
                    {
                        credit.CredValue += t.DebCredInValue;
                        await Credit_Edit_Service(credit);
                    } else if (t.DebCredChoice == credit.CredCode && credit.Hide == 1)
                    {
                        credit.Hide = 0;
                        await Credit_Edit_Service(credit);
                    }        
                }
            }
            if (t.DebCredInValue == 0 && t.DebCredChoice is not null)
            {
                var Debits = PersonalFinanceContext.Set<Debit>().AsNoTracking().AsQueryable().Where(x => x.Usr_OID == t.Usr_OID).ToList();
                var Credits = PersonalFinanceContext.Set<Credit>().AsNoTracking().AsQueryable().Where(x => x.Usr_OID == User_OID).ToList();

                if (t.DebCredChoice == "NDeb")
                {
                    foreach (var dd in Debits)
                    {
                        if (dd.DebCode == t.TrsCode)
                        {
                            await ExpToRemoveAsync(dd.DebCode, dd.Usr_OID, dd.Exp_ID);
                            await repo.DeleteDebitAsync(dd);
                            await repo.SaveChangesAsync();
                        }
                    }
                }
                else if (t.DebCredChoice == "NCred")
                {
                    foreach (var cc in Credits)
                    {
                        if (cc.CredCode == t.TrsCode)
                        {
                            await ExpToRemoveAsync(cc.CredCode, cc.Usr_OID, cc.Exp_ID);
                            await repo.DeleteCreditAsync(cc);
                            await repo.SaveChangesAsync();
                        }
                    }
                }
            }                  
            await repo.DeleteTransactionAsync(t);
            await repo.SaveChangesAsync();
            return Ok(t);
        }
    }
}