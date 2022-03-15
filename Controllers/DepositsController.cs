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
    public class DepositsController : PFA_APIController
    {
        private readonly PersonalFinanceContext PersonalFinanceContext;
        private readonly IRepository repo;
        public DepositsController(IRepository repo, PersonalFinanceContext PersonalFinanceContext) : base(repo)
        {
            this.repo = repo;
            this.PersonalFinanceContext = PersonalFinanceContext;
        }

        [HttpGet]
        [Route("All")]
        public async Task<IActionResult> Deposits_Main(string User_OID)
        {
            return Ok(await repo.GetAllDepositsAsync(User_OID));
        }
        [HttpGet]
        [Route("Details")]
        public async Task<IActionResult> Deposit_Details(int id, string User_OID)
        {
            var deposit = await repo.GetDepositAsync(id, User_OID);
            return Ok(deposit);
        }
        [HttpPost]
        [Route("Add")]
        public async Task<IActionResult> Deposit_Add([FromBody] Deposit d)
        {
            var detections = await repo.AddDepositAsync(d);
            await repo.SaveChangesAsync();
            return RedirectToAction(nameof(Deposits_Main));
        }
        [HttpPut]
        [Route("Update")]
        public async Task<IActionResult> Deposit_Edit(Deposit d)
        {
            await repo.UpdateDepositAsync(d);
            await repo.SaveChangesAsync();
            return Ok(d);
        }
        [HttpDelete]
        [Route("Delete")]
        public async Task<IActionResult> Deposit_Delete(int id, string User_OID)
        {
            var t = await repo.GetDepositAsync(id, User_OID);
            await repo.DeleteDepositAsync(t);
            await repo.SaveChangesAsync();
            return Ok(t);
        }
    }
}
