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

//Known Movements Controller
namespace PersonalFinance.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class WalletController : PFA_APIController
    {
        private readonly PersonalFinanceContext PersonalFinanceContext;
        private readonly IRepository repo;
        public WalletController(IRepository repo, PersonalFinanceContext PersonalFinanceContext) : base(repo)
        {
            this.repo = repo;
            this.PersonalFinanceContext = PersonalFinanceContext;
        }
        [HttpGet]
        [Route("All")]
        public async Task<IActionResult> Wallet_Main(string User_OID)
        {
            Wallet wallet = new();
            wallet.Banks = await repo.GetAllBanksAsync(User_OID);
            wallet.Deposits = await repo.GetAllDepositsAsync(User_OID);
            wallet.Tickets = await repo.GetAllTicketsAsync(User_OID);
            wallet.Contanti = wallet.Banks.First();
            return Ok(wallet);
        }



        [HttpDelete]
        [Route("DeleteBank")]
        public async Task<IActionResult> Bank_Delete(int id, string User_OID)
        {
            var t = await repo.GetBankAsync(id, User_OID);
            await repo.DeleteBankAsync(t);
            await repo.SaveChangesAsync();
            return Ok(t);
        }
        [HttpDelete]
        [Route("DeleteDeposit")]
        public async Task<IActionResult> Deposit_Delete(int id, string User_OID)
        {
            var t = await repo.GetDepositAsync(id, User_OID);
            await repo.DeleteDepositAsync(t);
            await repo.SaveChangesAsync();
            return Ok(t);
        }
        [HttpDelete]
        [Route("DeleteTicket")]
        public async Task<IActionResult> Ticket_Delete(int id, string User_OID)
        {
            var t = await repo.GetTicketAsync(id, User_OID);
            await repo.DeleteTicketAsync(t);
            await repo.SaveChangesAsync();
            return Ok(t);
        }
        [HttpPut]
        [Route("UpdateBank")]
        public async Task<IActionResult> Bank_Edit(Bank b)
        {
            await repo.UpdateBankAsync(b);
            await repo.SaveChangesAsync();
            return Ok(b);
        }
        [HttpPut]
        [Route("UpdateDeposit")]
        public async Task<IActionResult> Deposit_Edit(Deposit d)
        {
            await repo.UpdateDepositAsync(d);
            await repo.SaveChangesAsync();
            return Ok(d);
        }
        [HttpPut]
        [Route("UpdateTicket")]
        public async Task<IActionResult> Ticket_Edit(Ticket t)
        {
            await repo.UpdateTicketAsync(t);
            await repo.SaveChangesAsync();
            return Ok(t);
        }


    }
}
