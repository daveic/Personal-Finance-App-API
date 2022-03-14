using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PersonalFinance.Models;
using PersonalFinance.Services;

//Main API controller
namespace PersonalFinance.Controllers
{        













    [ApiController]
    [Route("api/[Controller]")]
    public class PersonalFinanceAPIController : Controller
    {



        private readonly IRepository repo;
        public PersonalFinanceAPIController(IRepository repo)
        {
            this.repo = repo;
        }

        
        //HTTP GET ALL METHODS
        //[HttpGet]
        //[Route("GetAllCredits")]
        //public async Task<IActionResult> AllCredits(string User_OID)
        //{
        //    var credits = await repo.GetAllCreditsAsync();
        //    credits = credits.Where(x => x.Usr_OID == User_OID);
        //    return Ok(credits);
        //}
        [HttpGet]
        [Route("GetAllDebits")]
        public async Task<IActionResult> AllDebits(string User_OID)
        {
            var debits = await repo.GetAllDebitsAsync();
            debits = debits.Where(x => x.Usr_OID == User_OID);
            return Ok(debits);
        }
        //[HttpGet]
        //[Route("GetAllKnownMovements")]
        //public async Task<IActionResult> AllKnownMovements(string User_OID)
        //{
        //    var knownMovements = await repo.GetAllKnownMovementsAsync();
        //    knownMovements = knownMovements.Where(x => x.Usr_OID == User_OID);
        //    return Ok(knownMovements);
        //}
        [HttpGet]
        [Route("GetAllTransactions")]
        public async Task<IActionResult> AllTransactions(string User_OID)
        {
            var transactions = await repo.GetAllTransactionsAsync();
            transactions = transactions.Where(x => x.Usr_OID == User_OID);
            return Ok(transactions);
        }
        [HttpGet]
        [Route("GetAllBanks")]
        public async Task<IActionResult> AllBanks(string User_OID)
        {
            var banks = await repo.GetAllBanksAsync(User_OID);
            
            return Ok(banks);
        }
        [HttpGet]
        [Route("GetAllDeposits")]
        public async Task<IActionResult> AllDeposits(string User_OID)
        {
            var deposits = await repo.GetAllDepositsAsync(User_OID);
           
            return Ok(deposits);
        }
        [HttpGet]
        [Route("GetAllTickets")]
        public async Task<IActionResult> AllTickets(string User_OID)
        {
            var tickets = await repo.GetAllTicketsAsync(User_OID);
    
            return Ok(tickets);
        }
        [HttpGet]
        [Route("GetAllBalances")]
        public async Task<IActionResult> AllBalances(string User_OID)
        {
            var balances = await repo.GetAllBalancesAsync();
            balances = balances.Where(x => x.Usr_OID == User_OID);
            return Ok(balances);
        }


        //HTTP GET BY ID METHODS
        //[HttpGet]
        //[Route("GetCreditId")]
        //public async Task<IActionResult> Credit_Details (int id, string User_OID)
        //{
        //    var credit = await repo.GetCreditAsync(id, User_OID);
        //    return Ok(credit);
        //}
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
        //[HttpGet]
        //[Route("GetKnownMovementId")]
        //public async Task<IActionResult> KnownMovement_Details(int id, string User_OID)
        //{
        //    var knownMovement = await repo.GetKnownMovementAsync(id);
        //    return Ok(knownMovement);
        //}
        [HttpGet]
        [Route("GetBankId")]
        public async Task<IActionResult> Bank_Details(int id, string User_OID)
        {
            var bank = await repo.GetBankAsync(id, User_OID);
            return Ok(bank);
        }
        [HttpGet]
        [Route("GetDepositId")]
        public async Task<IActionResult> Deposit_Details(int id, string User_OID)
        {
            var deposit = await repo.GetDepositAsync(id, User_OID);
            return Ok(deposit);
        }
        [HttpGet]
        [Route("GetTicketId")]
        public async Task<IActionResult> Ticket_Details(int id, string User_OID)
        {
            var ticket = await repo.GetTicketAsync(id, User_OID);
            return Ok(ticket);
        }


        //HTTP ADD METHODS

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
        [Route("AddBalance")]
        public async Task<IActionResult> AddBalance([FromBody] Balance b)
        {
            var detections = await repo.AddBalanceAsync(b);
            await repo.SaveChangesAsync();
            return RedirectToAction(nameof(AllBalances));
        }


        //HTTP DELETE METHODS

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


    }
}
