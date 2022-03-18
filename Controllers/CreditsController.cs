using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalFinance.Models;
using PersonalFinance.Services;
using PersonalFinance.Services.EntityFramework;

//Known Movements Controller
namespace PersonalFinance.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class CreditsController : PFA_APIController
    {
        private readonly PersonalFinanceContext PersonalFinanceContext;
        private readonly IRepository repo;
        public CreditsController(IRepository repo, PersonalFinanceContext PersonalFinanceContext) : base (repo)
        {
            this.repo = repo;
            this.PersonalFinanceContext = PersonalFinanceContext;
        }

        [HttpGet]
        [Route("Details")]
        public async Task<IActionResult> Credits_Details(int id, string User_OID)
        {
            var credit = await repo.GetCreditAsync(id, User_OID);
            return Ok(credit);
        }

        [HttpGet]
        [Route("All")]
        public async Task<IActionResult> Credits_Main(string User_OID)
        {            
            return Ok(await repo.GetAllCreditsAsync(User_OID));            
        }

        [HttpPost]
        [Route("Add")]
        public async Task<IActionResult> Credit_Add([FromBody] Credit c)
        {
            int i = await Credit_Add_Service(c);
            return RedirectToAction(nameof(Credits_Main));
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [NonAction]
        public async Task<int> Credit_Add_Service(Credit c)
        {
            Expiration exp = new()
            {
                Usr_OID = c.Usr_OID,
                ExpTitle = c.CredTitle,
                ExpDescription = "Rientro previsto - " + c.CredTitle,
                ExpDateTime = c.PrevDateTime,
                ColorLabel = "green",
                ExpValue = c.CredValue
            };
            await repo.AddExpirationAsync(exp);
            await repo.SaveChangesAsync();
            c.Exp_ID = PersonalFinanceContext.Set<Expiration>().AsNoTracking().AsQueryable().Where(x => x.Usr_OID == c.Usr_OID).OrderBy(x => x.ID).Last().ID;
            await repo.AddCreditAsync(c);
            await repo.SaveChangesAsync();
            return 1;
        }

        [HttpPut]
        [Route("Update")]
        public async Task<IActionResult> Credit_Edit(Credit c)
        {
            return Ok(await Credit_Edit_Service(c));
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [NonAction]
        public async Task<Credit> Credit_Edit_Service(Credit c)
        {
            var Expirations = PersonalFinanceContext.Set<Expiration>().AsNoTracking().AsQueryable().Where(x => x.Usr_OID == c.Usr_OID).ToList();
            foreach (var exp in Expirations)
            {
                if (c.Exp_ID == exp.ID)
                {
                    await repo.DeleteExpirationAsync(exp);
                    Expiration e = new()
                    {
                        Usr_OID = c.Usr_OID,
                        ExpTitle = c.CredTitle,
                        ExpDescription = "Rientro previsto - " + c.CredTitle,
                        ExpDateTime = c.PrevDateTime,
                        ColorLabel = "green",
                        ExpValue = c.CredValue
                    };
                    await repo.AddExpirationAsync(e);
                    await repo.SaveChangesAsync();
                    c.Exp_ID = Expirations.Last().ID + 1;
                    break;
                }
            }
            await repo.UpdateCreditAsync(c);
            await repo.SaveChangesAsync();
            return c;
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
