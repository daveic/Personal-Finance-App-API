using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PersonalFinance.Models;
using PersonalFinance.Services;
using PersonalFinance.Services.EntityFramework;

//Known Movements Controller
namespace PersonalFinance.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class BalanceController : PFA_APIController
    {
        private readonly PersonalFinanceContext PersonalFinanceContext;
        private readonly IRepository repo;
        public BalanceController(IRepository repo, PersonalFinanceContext PersonalFinanceContext) : base(repo)
        {
            this.repo = repo;
            this.PersonalFinanceContext = PersonalFinanceContext;
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
