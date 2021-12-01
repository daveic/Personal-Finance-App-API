using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PersonalFinance.Models;
using PersonalFinance.Services;

//Main API controller
namespace PersonalFinance.Controllers
{        
    [ApiController]
    [Route("api/[Controller]")]
    public class PersonalFinanceAPIController : ControllerBase
    {

        private readonly IRepository repo;
        public PersonalFinanceAPIController(IRepository repo)
        {
            this.repo = repo;
        }

        //HTTP GET ALL METHODS
        [HttpGet]
        [Route("GetAllCredits")]
        public async Task<IActionResult> AllCredits()
        {
            var credits = await repo.GetAllCreditsAsync();
            return Ok(credits);
        }
        [HttpGet]
        [Route("GetAllDebits")]
        public async Task<IActionResult> AllDebits()
        {
            var debits = await repo.GetAllDebitsAsync();
            return Ok(debits);
        }
        [HttpGet]
        [Route("GetAllKnownMovements")]
        public async Task<IActionResult> AllKnownMovements()
        {
            var knownMovements = await repo.GetAllKnownMovementsAsync();
            return Ok(knownMovements);
        }
        [HttpGet]
        [Route("GetAllTransactions")]
        public async Task<IActionResult> AllTransactions()
        {
            var transactions = await repo.GetAllTransactionsAsync();
            return Ok(transactions);
        }
        [HttpGet]
        [Route("GetAllBanks")]
        public async Task<IActionResult> AllBanks()
        {
            var banks = await repo.GetAllBanksAsync();
            return Ok(banks);
        }
        [HttpGet]
        [Route("GetAllDeposits")]
        public async Task<IActionResult> AllDeposits()
        {
            var deposits = await repo.GetAllDepositsAsync();
            return Ok(deposits);
        }
        [HttpGet]
        [Route("GetAllTickets")]
        public async Task<IActionResult> AllTickets()
        {
            var tickets = await repo.GetAllTicketsAsync();
            return Ok(tickets);
        }

        //HTTP GET BY ID METHODS
        [HttpGet]
        [Route("GetCreditId")]
        public async Task<IActionResult> Credit_Details (int id)
        {
            var credit = await repo.GetCreditAsync(id);
            return Ok(credit);
        }
        [HttpGet]
        [Route("GetDebitId")]
        public async Task<IActionResult> Debit_Details(int id)
        {
            var debit = await repo.GetDebitAsync(id);
            return Ok(debit);
        }
        [HttpGet]
        [Route("GetTransactionId")]
        public async Task<IActionResult> Transaction_Details(int id)
        {
            var transaction = await repo.GetTransactionAsync(id);
            return Ok(transaction);
        }
        [HttpGet]
        [Route("GetKnownMovementId")]
        public async Task<IActionResult> KnownMovement_Details(int id)
        {
            var knownMovement = await repo.GetKnownMovementAsync(id);
            return Ok(knownMovement);
        }
        [HttpGet]
        [Route("GetBankId")]
        public async Task<IActionResult> Bank_Details(int id)
        {
            var bank = await repo.GetBankAsync(id);
            return Ok(bank);
        }
        [HttpGet]
        [Route("GetDepositId")]
        public async Task<IActionResult> Deposit_Details(int id)
        {
            var deposit = await repo.GetDepositAsync(id);
            return Ok(deposit);
        }
        [HttpGet]
        [Route("GetTicketId")]
        public async Task<IActionResult> Ticket_Details(int id)
        {
            var ticket = await repo.GetTicketAsync(id);
            return Ok(ticket);
        }

        //HTTP ADD METHODS
        [HttpPost]
        [Route("AddCredit")]
        public async Task<IActionResult> AddCredit([FromBody] Credit c)
        {
            var credits = await repo.AddCreditAsync(c);
            await repo.SaveChangesAsync();
            return RedirectToAction(nameof(AllCredits));
        }
        [HttpPost]
        [Route("AddDebit")]
        public async Task<IActionResult> AddDebit([FromBody] Debit d)
        {
            var detections = await repo.AddDebitAsync(d);
            await repo.SaveChangesAsync();
            return RedirectToAction(nameof(AllDebits));
        }
        [HttpPost]
        [Route("AddTransaction")]
        public async Task<IActionResult> AddTransaction([FromBody] Transaction t)
        {
            var detections = await repo.AddTransactionAsync(t);
            await repo.SaveChangesAsync();
            return RedirectToAction(nameof(AllTransactions));
        }
        [HttpPost]
        [Route("AddKnownMovement")]
        public async Task<IActionResult> AddKnownMovement([FromBody] KnownMovement k)
        {
            var detections = await repo.AddKnownMovementAsync(k);
            await repo.SaveChangesAsync();
            return RedirectToAction(nameof(AllKnownMovements));
        }
        [HttpPost]
        [Route("AddBank")]
        public async Task<IActionResult> AddBank([FromBody] Bank b)
        {
            var detections = await repo.AddBankAsync(b);
            await repo.SaveChangesAsync();
            return RedirectToAction(nameof(AllBanks));
        }
        [HttpPost]
        [Route("AddDeposit")]
        public async Task<IActionResult> AddDeposit([FromBody] Deposit d)
        {
            var detections = await repo.AddDepositAsync(d);
            await repo.SaveChangesAsync();
            return RedirectToAction(nameof(AllDeposits));
        }
        [HttpPost]
        [Route("AddTicket")]
        public async Task<IActionResult> AddTicket([FromBody] Ticket t)
        {
            var detections = await repo.AddTicketAsync(t);
            await repo.SaveChangesAsync();
            return RedirectToAction(nameof(AllTickets));
        }

        //HTTP DELETE METHODS
        [HttpDelete]
        [Route("DeleteCredit")]
        public async Task<IActionResult> Credit_Delete(int id)
        {
            var t = await repo.GetCreditAsync(id);
            await repo.DeleteCreditAsync(t);
            await repo.SaveChangesAsync();
            return Ok(t);
        }
        [HttpDelete]
        [Route("DeleteDebit")]
        public async Task<IActionResult> Debit_Delete(int id)
        {
            var t = await repo.GetDebitAsync(id);
            await repo.DeleteDebitAsync(t);
            await repo.SaveChangesAsync();
            return Ok(t);
        }
        [HttpDelete]
        [Route("DeleteTransaction")]
        public async Task<IActionResult> Transaction_Delete(int id)
        {
            var t = await repo.GetTransactionAsync(id);
            await repo.DeleteTransactionAsync(t);
            await repo.SaveChangesAsync();
            return Ok(t);
        }
        [HttpDelete]
        [Route("DeleteKnownMovement")]
        public async Task<IActionResult> KnownMovement_Delete(int id)
        {
            var t = await repo.GetKnownMovementAsync(id);
            await repo.DeleteKnownMovementAsync(t);
            await repo.SaveChangesAsync();
            return Ok(t);
        }
        [HttpDelete]
        [Route("DeleteBank")]
        public async Task<IActionResult> Bank_Delete(int id)
        {
            var t = await repo.GetBankAsync(id);
            await repo.DeleteBankAsync(t);
            await repo.SaveChangesAsync();
            return Ok(t);
        }
        [HttpDelete]
        [Route("DeleteDeposit")]
        public async Task<IActionResult> Deposit_Delete(int id)
        {
            var t = await repo.GetDepositAsync(id);
            await repo.DeleteDepositAsync(t);
            await repo.SaveChangesAsync();
            return Ok(t);
        }
        [HttpDelete]
        [Route("DeleteTicket")]
        public async Task<IActionResult> Ticket_Delete(int id)
        {
            var t = await repo.GetTicketAsync(id);
            await repo.DeleteTicketAsync(t);
            await repo.SaveChangesAsync();
            return Ok(t);
        }

        //HTTP UPDATE METHODS
        [HttpPut]
        [Route("UpdateCredit")]
        public async Task<IActionResult> Credit_Edit(Credit c)
        {
           await repo.UpdateCreditAsync(c);
           await repo.SaveChangesAsync();
           return Ok(c);
        }
        [HttpPut]
        [Route("UpdateDebit")]
        public async Task<IActionResult> Debit_Edit(Debit d)
        {
            await repo.UpdateDebitAsync(d);
            await repo.SaveChangesAsync();
            return Ok(d);
        }
        [HttpPut]
        [Route("UpdateTransaction")]
        public async Task<IActionResult> Transaction_Edit(Transaction t)
        {
            await repo.UpdateTransactionAsync(t);
            await repo.SaveChangesAsync();
            return Ok(t);
        }
        [HttpPut]
        [Route("UpdateKnownMovement")]
        public async Task<IActionResult> KnownMovement_Edit(KnownMovement k)
        {
            await repo.UpdateKnownMovementAsync(k);
            await repo.SaveChangesAsync();
            return Ok(k);
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
