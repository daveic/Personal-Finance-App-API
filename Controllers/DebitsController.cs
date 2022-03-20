using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalFinance.Models;
using PersonalFinance.Services;
using PersonalFinance.Services.EntityFramework;

//Debits Controller
namespace PersonalFinance.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class DebitsController : PFA_APIController
    {
        private readonly PersonalFinanceContext PersonalFinanceContext;
        private readonly IRepository repo;
        public DebitsController(IRepository repo, PersonalFinanceContext PersonalFinanceContext) : base (repo, PersonalFinanceContext)
        {
            this.repo = repo;
            this.PersonalFinanceContext = PersonalFinanceContext;
        }

        [HttpGet]
        [Route("All")]
        public async Task<IActionResult> Debits_Main(string User_OID)
        {
            return Ok(await repo.GetAllDebitsAsync(User_OID));
        }
        [HttpGet]
        [Route("Details")]
        public async Task<IActionResult> Debit_Details(int id, string User_OID)
        {
            return Ok(await repo.GetDebitAsync(id, User_OID));
        }
        [HttpPost]
        [Route("Add")]
        public async Task<IActionResult> Debit_Add([FromBody] Debit d)
        {
            await Debit_Add_Service(d);
            return RedirectToAction(nameof(Debits_Main));
        }
        [HttpPut]
        [Route("Update")]
        public async Task<IActionResult> Debit_Edit(Debit d)
        {
            return Ok(await Debit_Edit_Service(d));
        }
        [HttpDelete]
        [Route("Delete")]
        public async Task<IActionResult> Debit_Delete(int id, string User_OID)
        {
            Debit deb = PersonalFinanceContext.Set<Debit>().AsNoTracking().AsQueryable().Where(x => x.Usr_OID == User_OID).FirstOrDefault(x => x.ID == id);
            await ExpToRemoveAsync(deb.DebCode, User_OID, deb.Exp_ID);
            var t = await repo.GetDebitAsync(id, User_OID);
            await repo.DeleteDebitAsync(t);
            await repo.SaveChangesAsync();
            return Ok(t);
        }
    }
}