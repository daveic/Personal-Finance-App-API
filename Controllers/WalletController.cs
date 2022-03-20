using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PersonalFinance.Models;
using PersonalFinance.Services;
using PersonalFinance.Services.EntityFramework;

//Wallet Controller
namespace PersonalFinance.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class WalletController : PFA_APIController
    {
        private readonly IRepository repo;
        public WalletController(IRepository repo, PersonalFinanceContext PersonalFinanceContext) : base(repo, PersonalFinanceContext)
        {
            this.repo = repo;
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
    }
}
