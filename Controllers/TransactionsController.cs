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
    public class TrabsactionsController : PFA_APIController
    {
        private readonly PersonalFinanceContext PersonalFinanceContext;
        private readonly IRepository repo;
        public TrabsactionsController(IRepository repo, PersonalFinanceContext PersonalFinanceContext) : base(repo)
        {
            this.repo = repo;
            this.PersonalFinanceContext = PersonalFinanceContext;
        }
        [HttpGet]
        [Route("Details")]
        public async Task<IActionResult> Transaction_Details(int id)
        {
            return Ok(await repo.GetTransactionAsync(id));
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
        [HttpDelete]
        [Route("Delete")]
        public async Task<IActionResult> Transaction_Delete(int id)
        {
            var t = await repo.GetTransactionAsync(id);
            await repo.DeleteTransactionAsync(t);
            await repo.SaveChangesAsync();
            return Ok(t);
        }


    }
}
