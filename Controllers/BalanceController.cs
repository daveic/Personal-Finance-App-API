using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalFinance.Models;
using PersonalFinance.Services;
using PersonalFinance.Services.EntityFramework;

//Known Movements Controller
namespace PersonalFinance.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class BalancesController : PFA_APIController
    {
        private readonly PersonalFinanceContext PersonalFinanceContext;
        private readonly IRepository repo;
        public BalancesController(IRepository repo, PersonalFinanceContext PersonalFinanceContext) : base(repo)
        {
            this.repo = repo;
            this.PersonalFinanceContext = PersonalFinanceContext;
        }


        [HttpGet]
        [Route("All")]
        public async Task<IActionResult> Balances_All(string User_OID)
        {
            return Ok(await repo.GetAllBalancesAsync(User_OID));
        }


        [HttpPost]
        [Route("Update")]
        public async Task<IActionResult> Balance_Update(string User_OID)
        {
            Balance b = new();
            b.Usr_OID = User_OID;
            b.BalDateTime = DateTime.UtcNow;
            IEnumerable<Transaction> Transactions = PersonalFinanceContext.Set<Transaction>().AsNoTracking().AsQueryable().Where(x => x.Usr_OID == User_OID).ToList();
            IEnumerable<Bank> Banks = PersonalFinanceContext.Set<Bank>().AsNoTracking().AsQueryable().Where(x => x.Usr_OID == User_OID).ToList();
            IEnumerable<Ticket> Tickets = PersonalFinanceContext.Set<Ticket>().AsNoTracking().AsQueryable().Where(x => x.Usr_OID == User_OID).ToList();
            double tot = 0;
            double totTransaction = 0;

            foreach (var item in Banks)
            {
                tot += item.BankValue;
            }
            foreach (var item in Tickets)
            {
                tot += Convert.ToInt32(item.NumTicket) * item.TicketValue;
            }
            foreach (var item in Transactions)
            {
                totTransaction += item.TrsValue;
            }

            Transaction tr = new() { Usr_OID = User_OID, TrsCode = "Fast_Update", TrsTitle = "Allineamento Fast Update", TrsDateTime = DateTime.UtcNow, TrsValue = tot - totTransaction, TrsNote = "Allineamento Fast Update eseguito il " + DateTime.UtcNow };
            await repo.AddTransactionAsync(tr);
            await repo.SaveChangesAsync();
            b.ActBalance = tot;

            await repo.AddBalanceAsync(b);
            await repo.SaveChangesAsync();
            return Ok(1);
        }




        [HttpPost]
        [Route("Add")]
        public async Task<IActionResult> AddBalance([FromBody] Balance b)
        {
            await repo.AddBalanceAsync(b);
            await repo.SaveChangesAsync();
            return RedirectToAction(nameof(Balances_All));
        }


    }
}
