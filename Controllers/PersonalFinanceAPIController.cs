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
        [Route("GetTransactionId")]
        public async Task<IActionResult> Transaction_Details(int id)
          {
              var transaction = await repo.GetTransactionAsync(id);
              return Ok(transaction);
        }






        //HTTP ADD METHODS


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
