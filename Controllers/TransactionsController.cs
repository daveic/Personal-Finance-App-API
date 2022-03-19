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

//Known Movements Controller
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
            IEnumerable<Debit> Debits = await repo.GetAllDebitsAsync(User_OID);
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
            var detections = await repo.AddTransactionAsync(t);
            await repo.SaveChangesAsync();
            await Transaction_Credit_Debit_UpdateAsync(t);
            return RedirectToAction(nameof(Transactions_Main));
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [NonAction]
        public async Task<int> Credit_Add_Service([FromBody] Credit c)
        {
            Expiration exp = new()
            {
                Usr_OID = c.Usr_OID,
                ExpTitle = c.CredTitle,
                ExpDescription = "Rientro previsto - " + c.CredTitle,
                ExpDateTime = c.PrevDateTime,
                ColorLabel = "green",
                ExpValue = c.CredValue
            };
            await repo.AddExpirationAsync(exp);
            await repo.SaveChangesAsync();
            c.Exp_ID = PersonalFinanceContext.Set<Expiration>().AsNoTracking().AsQueryable().Where(x => x.Usr_OID == c.Usr_OID).OrderBy(x => x.ID).Last().ID;
            await repo.AddCreditAsync(c);
            await repo.SaveChangesAsync();
            return 1;
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [NonAction]
        public async Task<Credit> Credit_Edit_Service(Credit c)
        {
            var Expirations = PersonalFinanceContext.Set<Expiration>().AsNoTracking().AsQueryable().Where(x => x.Usr_OID == c.Usr_OID).ToList();
            foreach (var exp in Expirations)
            {
                if (c.Exp_ID == exp.ID)
                {
                    await repo.DeleteExpirationAsync(exp);
                    Expiration e = new()
                    {
                        Usr_OID = c.Usr_OID,
                        ExpTitle = c.CredTitle,
                        ExpDescription = "Rientro previsto - " + c.CredTitle,
                        ExpDateTime = c.PrevDateTime,
                        ColorLabel = "green",
                        ExpValue = c.CredValue
                    };
                    await repo.AddExpirationAsync(e);
                    c.Exp_ID = Expirations.Last().ID + 1;
                    break;
                }
            }
            await repo.UpdateCreditAsync(c);
            await repo.SaveChangesAsync();
            return c;
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [NonAction]
        public async Task<int> Debit_Add_Service(Debit d)
        {
            if (d.DebDateTime == DateTime.MinValue)
            {
                d.DebDateTime = d.DebInsDate.AddMonths(Convert.ToInt32((d.RtNum * d.Multiplier)));
            }

            for (int k = 0; k < d.RtNum; k++)
            {
                Expiration exp = new()
                {
                    Usr_OID = d.Usr_OID,
                    ExpTitle = d.DebTitle,
                    ExpDescription = d.DebTitle + "rata: " + (k + 1)
                };
                if (d.RtFreq == "Mesi")
                {
                    exp.ExpDateTime = d.DebInsDate.AddMonths(k * d.Multiplier);
                }
                if (d.RtFreq == "Anni")
                {
                    exp.ExpDateTime = d.DebInsDate.AddYears(k * d.Multiplier);
                }
                exp.ColorLabel = "red";
                exp.ExpValue = d.DebValue / d.RtNum;
                await repo.AddExpirationAsync(exp);
            }
            await repo.SaveChangesAsync();
            d.Exp_ID = PersonalFinanceContext.Set<Expiration>().AsNoTracking().AsQueryable().Where(x => x.Usr_OID == d.Usr_OID).OrderBy(x => x.ID).Last().ID - Convert.ToInt32(d.RtNum) + 1;
            var detections = await repo.AddDebitAsync(d);
            await repo.SaveChangesAsync();
            return 1;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [NonAction]
        public async Task<int> Transaction_Credit_Debit_UpdateAsync(Transaction t)
        {

            //IEnumerable<Credit> Credits = await repo.GetAllCreditsAsync(t.Usr_OID);
            //IEnumerable<Debit> Debits = await repo.GetAllDebitsAsync(t.Usr_OID);
            var Credits = PersonalFinanceContext.Set<Credit>().AsNoTracking().AsQueryable().Where(x => x.Usr_OID == t.Usr_OID).ToList();
            var Debits = PersonalFinanceContext.Set<Debit>().AsNoTracking().AsQueryable().Where(x => x.Usr_OID == t.Usr_OID).ToList();

            if (t.TrsValue < 0)
            {
                foreach (var debit in Debits)
                {
                    if (t.TrsCode == debit.DebCode)
                    {
                        debit.RemainToPay += t.TrsValue;
                        debit.RtPaid += (-t.TrsValue) / (debit.DebValue / debit.RtNum);
                        var exp = await repo.GetExpirationAsync((debit.Exp_ID + Convert.ToInt32(debit.RtPaid - 1)), debit.Usr_OID);
                        await repo.DeleteExpirationAsync(exp);
                        await repo.SaveChangesAsync();

                        if (debit.RemainToPay <= 0)
                        {
                            await repo.DeleteDebitAsync(debit);
                            await repo.SaveChangesAsync();
                        }
                        else
                        {
                            
                          //  Debit_Edit(debit, 1, true);
                           // Debit_Edit_Service(Debit_Exp dexp)
                        }
                    }

                }
                if (t.TrsCode.StartsWith("CRE"))
                {
                    Credit model = new()
                    {
                        Usr_OID = t.Usr_OID,
                        CredCode = t.TrsCode,
                        CredDateTime = DateTime.UtcNow,
                        CredValue = t.TrsValue,
                        CredTitle = "Prestito/Anticipo",
                        CredNote = ""
                    };
                    await Credit_Add_Service(model);
                }

            }
            if (t.TrsValue > 0)
            {
                foreach (var credit in Credits)
                {
                    if (t.TrsCode == credit.CredCode)
                    {
                        credit.CredValue -= t.TrsValue;
                        if (credit.CredValue <= 0)
                        {
                            await repo.DeleteCreditAsync(credit);
                            await repo.SaveChangesAsync();
                        }
                        else
                        {
                            await Credit_Edit_Service(credit);
                            //Credit_Edit(credit, 1);
                        }
                    }
                }
                if (t.TrsCode.StartsWith("DEB"))
                {
                    Debit model = new();
                    model.Usr_OID = t.Usr_OID;
                    model.DebCode = t.TrsCode;
                    model.DebInsDate = DateTime.UtcNow;
                    model.DebValue = -t.TrsValue;
                    model.DebTitle = "Prestito/Anticipo";
                    model.DebNote = "";
                    model.RemainToPay = -t.TrsValue;
                    model.RtPaid = 0;
                    model.RtNum = 1;
                    await Debit_Add_Service(model);
                    //Debit_Add(model, 1);
                }
            }
            return 1;
        }
        [HttpGet]
        [Route("DetailsEdit")]
        public IActionResult Transaction_Details_Edit(TransactionDetailsEdit TrDet)
        {
            var UniqueCodes = PersonalFinanceContext.Set<Transaction>().AsNoTracking().AsQueryable()
                .Where(x => x.Usr_OID == TrDet.User_OID)
                .GroupBy(x => x.TrsCode)
                .Select(x => x.First())
                .ToList();
            //IEnumerable<Transaction> Transactions = GetAllItems<Transaction>("PersonalFinanceAPI", nameof(Transactions), User_OID);
            var Credits = PersonalFinanceContext.Set<Credit>().AsNoTracking().AsQueryable().Where(x => x.Usr_OID == TrDet.User_OID).ToList();
            var Debits = PersonalFinanceContext.Set<Debit>().AsNoTracking().AsQueryable().Where(x => x.Usr_OID == TrDet.User_OID).ToList();
            //IEnumerable<Credit> Credits = GetAllItems<Credit>("PersonalFinanceAPI", nameof(Credits), User_OID);
            //IEnumerable<Debit> Debits = GetAllItems<Debit>("PersonalFinanceAPI", nameof(Debits), User_OID);
            //Transaction t = GetItemID<Transaction>(nameof(Transaction), id);
            /*var UniqueCodes = Transactions.GroupBy(x => x.TrsCode)
                                          .Select(x => x.First())
                                          .ToList();*/
            TrDet.Codes = new List<SelectListItem>();
            foreach (var item in UniqueCodes)
            {
                SelectListItem code = new();
                code.Value = item.TrsCode;
                code.Text = item.TrsCode;
                TrDet.Codes.Add(code);
            }
            bool isPresent = false;
            foreach (var credit in Credits)
            {
                foreach (var item in TrDet.Codes)
                {
                    if (credit.CredCode == item.Value) isPresent = true;
                }
                if (isPresent is true)
                {
                    SelectListItem code = new()
                    {
                        Value = credit.CredCode,
                        Text = credit.CredCode
                    };
                    TrDet.Codes.Add(code);
                }
                isPresent = false;
            }
            foreach (var debit in Debits)
            {
                foreach (var item in TrDet.Codes)
                {
                    if (debit.DebCode == item.Value) isPresent = true;
                }
                if (isPresent is false)
                {
                    SelectListItem code = new()
                    {
                        Value = debit.DebCode,
                        Text = debit.DebCode
                    };
                    TrDet.Codes.Add(code);
                }
                isPresent = false;
            }
            return Ok(TrDet.Codes);
        }
        [HttpPut]
        [Route("Update")]
        public async Task<IActionResult> Transaction_Edit(Transaction t)
        {
            await repo.UpdateTransactionAsync(t);
            await repo.SaveChangesAsync();
            return Ok(t);
        }
        [HttpDelete]
        [Route("Delete")]
        public async Task<IActionResult> Transaction_Delete(int id, string User_OID)
        {
            var t = await repo.GetTransactionAsync(id, User_OID);
            await repo.DeleteTransactionAsync(t);
            await repo.SaveChangesAsync();
            return Ok(t);
        }


    }
}
