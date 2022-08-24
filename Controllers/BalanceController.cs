using System;
using System.Collections.Generic;
using System.Linq;
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
        public BalancesController(IRepository repo, PersonalFinanceContext PersonalFinanceContext) : base(repo, PersonalFinanceContext)
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
        [Route("Add")]
        public async Task<IActionResult> Balance_Update(Balance b)
        {
            IEnumerable<Transaction> Transactions = PersonalFinanceContext.Set<Transaction>().AsNoTracking().AsQueryable().Where(x => x.Usr_OID == b.Usr_OID).ToList();
            IEnumerable<Bank> Banks = PersonalFinanceContext.Set<Bank>().AsNoTracking().AsQueryable().Where(x => x.Usr_OID == b.Usr_OID).ToList();
            IEnumerable<Ticket> Tickets = PersonalFinanceContext.Set<Ticket>().AsNoTracking().AsQueryable().Where(x => x.Usr_OID == b.Usr_OID).ToList();
            double tot = 0;
            double totTransaction = 0;
           
            foreach (var item in Transactions)
            {
                totTransaction += item.TrsValue;
            }
            if( b.FromFU is true)
            {
                foreach (var item in Banks)
                {
                    tot += item.BankValue;
                }
                foreach (var item in Tickets)
                {
                    tot += Convert.ToInt32(item.NumTicket) * item.TicketValue;
                }
                Transaction tr = new() { Usr_OID = b.Usr_OID, TrsCode = "Fast_Update", TrsTitle = "Allineamento Fast Update", TrsDateTime = DateTime.UtcNow, TrsValue = tot - totTransaction, TrsNote = "Allineamento Fast Update eseguito il " + DateTime.UtcNow };
                await repo.AddTransactionAsync(tr);
                await repo.SaveChangesAsync();
                b.ActBalance = tot;
            }
            b.ActBalance = totTransaction;
            await repo.AddBalanceAsync(b);
            await repo.SaveChangesAsync();
            return Ok(1);
        }
    }
}
