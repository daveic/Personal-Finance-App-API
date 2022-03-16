using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PersonalFinance.Models;
using PersonalFinance.Services;
using PersonalFinance.Services.EntityFramework;

//Known Movements Controller
namespace PersonalFinance.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class TransactionsController : PFA_APIController
    {
        private readonly PersonalFinanceContext PersonalFinanceContext;
        private readonly IRepository repo;
        public TransactionsController(IRepository repo, PersonalFinanceContext PersonalFinanceContext) : base(repo)
        {
            this.repo = repo;
            this.PersonalFinanceContext = PersonalFinanceContext;
        }

        [HttpGet]
        [Route("All")]
        public async Task<IActionResult> Transactions_Main(string User_OID)
        {
            IEnumerable<Transaction> Transactions = await repo.GetAllTransactionsAsync(User_OID);
            IEnumerable<Credit> Credits = await repo.GetAllCreditsAsync(User_OID);
            IEnumerable<Debit> Debits = await repo.GetAllDebitsAsync(User_OID);
            Transactions Trs = new ();

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
        public async Task<IActionResult> AddTransaction([FromBody] Transaction t)
        {
            var detections = await repo.AddTransactionAsync(t);
            await repo.SaveChangesAsync();
            return RedirectToAction(nameof(Transactions_Main));
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
