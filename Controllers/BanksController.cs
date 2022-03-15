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
    public class BanksController : PFA_APIController
    {
        private readonly PersonalFinanceContext PersonalFinanceContext;
        private readonly IRepository repo;
        public BanksController(IRepository repo, PersonalFinanceContext PersonalFinanceContext) : base(repo)
        {
            this.repo = repo;
            this.PersonalFinanceContext = PersonalFinanceContext;
        }


        [HttpGet]
        [Route("All")]
        public async Task<IActionResult> Banks_Main(string User_OID)
        {
            var banks = await repo.GetAllBanksAsync(User_OID);

            return Ok(banks);
        }
        [HttpGet]
        [Route("Details")]
        public async Task<IActionResult> Bank_Details(int id, string User_OID)
        {
            var bank = await repo.GetBankAsync(id, User_OID);
            return Ok(bank);
        }
        [HttpPost]
        [Route("Add")]
        public async Task<IActionResult> Bank_Add([FromBody] Bank b)
        {
            var detections = await repo.AddBankAsync(b);
            await repo.SaveChangesAsync();
            return RedirectToAction(nameof(Banks_Main));
        }
        [HttpPut]
        [Route("Update")]
        public async Task<IActionResult> Bank_Edit(Bank b)
        {
            await repo.UpdateBankAsync(b);
            await repo.SaveChangesAsync();
            return Ok(b);
        }
        [HttpDelete]
        [Route("Delete")]
        public async Task<IActionResult> Bank_Delete(int id, string User_OID)
        {
            var t = await repo.GetBankAsync(id, User_OID);
            await repo.DeleteBankAsync(t);
            await repo.SaveChangesAsync();
            return Ok(t);
        }


    }
}
