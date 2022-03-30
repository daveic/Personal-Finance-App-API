using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalFinance.Models;
using PersonalFinance.Services;
using PersonalFinance.Services.EntityFramework;

//Credits Controller
namespace PersonalFinance.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class CreditsController : PFA_APIController
    {
        private readonly PersonalFinanceContext PersonalFinanceContext;
        private readonly IRepository repo;
        public CreditsController(IRepository repo, PersonalFinanceContext PersonalFinanceContext) : base (repo, PersonalFinanceContext)
        {
            this.repo = repo;
            this.PersonalFinanceContext = PersonalFinanceContext;
        }

        [HttpGet]
        [Route("All")]
        public async Task<IActionResult> Credits_Main(string User_OID)
        {            
            return Ok(await repo.GetAllCreditsAsync(User_OID));            
        }
        [HttpGet]
        [Route("Details")]
        public async Task<IActionResult> Credits_Details(int id, string User_OID)
        {
            return Ok(await repo.GetCreditAsync(id, User_OID));
        }
        [HttpPost]
        [Route("Add")]
        public async Task<IActionResult> Credit_Add([FromBody] Credit c)
        {
            c.CredCode = "CRE " + c.CredTitle;
            int i = await Credit_Add_Service(c);
            return RedirectToAction(nameof(Credits_Main));
        }
        [HttpPut]
        [Route("Update")]
        public async Task<IActionResult> Credit_Edit(Credit c)
        {
            return Ok(await Credit_Edit_Service(c));
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [NonAction]
        public async Task<IActionResult> Post()
        {
            bool v = await PersonalFinanceContext.SaveChangesAsync() > 0;
            return Ok(v);
        }

        [HttpDelete]
        [Route("Delete")]
        public async Task<IActionResult> Credit_Delete(int id, string User_OID)
        {
            var credit = await repo.GetCreditAsync(id, User_OID);
            Expiration e = PersonalFinanceContext.Set<Expiration>().AsNoTracking().AsQueryable().Where(x => x.Usr_OID == User_OID).FirstOrDefault(x => x.ID == credit.Exp_ID);        
            await repo.DeleteExpirationAsync(e);
            await repo.DeleteCreditAsync(credit);
            await repo.SaveChangesAsync();
            return Ok(credit);
        }
    }
}
