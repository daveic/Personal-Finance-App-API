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
        [Route("GetAllBalances")]
        public async Task<IActionResult> AllBalances(string User_OID)
        {
            return Ok(await repo.GetAllBalancesAsync(User_OID));
        }







        [HttpPost]
        [Route("AddBalance")]
        public async Task<IActionResult> AddBalance([FromBody] Balance b)
        {
            var detections = await repo.AddBalanceAsync(b);
            await repo.SaveChangesAsync();
            return RedirectToAction(nameof(AllBalances));
        }


   











    }
}
