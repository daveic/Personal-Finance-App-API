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


        [HttpGet]
        [Route("GetAllDebits")]
        public async Task<IActionResult> AllDebits(string User_OID)
        {
            var debits = await repo.GetAllDebitsAsync();
            debits = debits.Where(x => x.Usr_OID == User_OID);
            return Ok(debits);
        }

        [HttpGet]
        [Route("GetAllTransactions")]
        public async Task<IActionResult> AllTransactions(string User_OID)
        {
            var transactions = await repo.GetAllTransactionsAsync();
            transactions = transactions.Where(x => x.Usr_OID == User_OID);
            return Ok(transactions);
        }



        [HttpGet]
        [Route("GetAllBalances")]
        public async Task<IActionResult> AllBalances(string User_OID)
        {
            var balances = await repo.GetAllBalancesAsync();
            balances = balances.Where(x => x.Usr_OID == User_OID);
            return Ok(balances);
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
