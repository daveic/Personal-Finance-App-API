using System;
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
        public async Task<IActionResult> AddCredit([FromBody] Credit c)
        {
            Expiration exp = new Expiration();
            exp.Usr_OID = c.Usr_OID;
            exp.ExpTitle = c.CredTitle;
            exp.ExpDescription = "Rientro previsto - " + c.CredTitle;
            exp.ExpDateTime = c.PrevDateTime;
            exp.ColorLabel = "green";
            exp.ExpValue = c.CredValue;
            await repo.AddExpirationAsync(exp);
            await repo.SaveChangesAsync();
            c.Exp_ID = PersonalFinanceContext.Set<Expiration>().AsNoTracking().AsQueryable().Where(x => x.Usr_OID == c.Usr_OID).OrderBy(x => x.ID).Last().ID;
            await repo.AddCreditAsync(c);
            await repo.SaveChangesAsync();
            return RedirectToAction(nameof(Credits_Main));
        }

        [HttpPut]
        [Route("Update")]
        public async Task<IActionResult> Credit_Edit(Credit c)
        {
            var Expirations = PersonalFinanceContext.Set<Expiration>().AsNoTracking().AsQueryable().Where(x => x.Usr_OID == c.Usr_OID).ToList();            
            foreach (var exp in Expirations)
            {
                if (c.Exp_ID == exp.ID)
                {
                    await repo.DeleteExpirationAsync(exp);
                    Expiration e = new Expiration();
                    e.Usr_OID = c.Usr_OID;
                    e.ExpTitle = c.CredTitle;
                    e.ExpDescription = "Rientro previsto - " + c.CredTitle;
                    e.ExpDateTime = c.PrevDateTime;
                    e.ColorLabel = "green";
                    e.ExpValue = c.CredValue;
                    await repo.AddExpirationAsync(e);
                    c.Exp_ID = Expirations.Last().ID + 1;
                    break;
                }
            }
            await repo.UpdateCreditAsync(c);
            await repo.SaveChangesAsync();
            return Ok(c);
        }


        [HttpDelete]
        [Route("Delete")]
        public async Task<IActionResult> Credit_Delete(int id, string User_OID)
        {
            var credit = await repo.GetCreditAsync(id, User_OID);
            Expiration e = PersonalFinanceContext.Set<Expiration>().AsNoTracking().AsQueryable().Where(x => x.Usr_OID == User_OID).FirstOrDefault(x => x.ID == credit.Exp_ID);
            this.PersonalFinanceContext.Remove(e);

           // await repo.SaveChangesAsync();


            var t = await repo.GetCreditAsync(id, User_OID);
            await repo.DeleteCreditAsync(t);
            await repo.SaveChangesAsync();
            return Ok(t);
        }
        //////HTTP DELETE METHODS

        //[HttpDelete]
        //[Route("DeleteKnownMovement")]
        //public async Task<IActionResult> KnownMovement_Delete(int id)
        //{
        //    var km = await repo.GetKnownMovementAsync(id);
        //    await ExpToRemoveAsync(km.KMTitle, km.Usr_OID, km.Exp_ID);
        //    await repo.DeleteKnownMovementAsync(km);
        //    await repo.SaveChangesAsync();
        //    return Ok(km);
        //}


        ////HTTP UPDATE METHODS

        //[HttpPut]
        //[Route("UpdateKnownMovement")]
        //public async Task<IActionResult> KnownMovement_EditAsync(KnownMovement k)
        //{
        //    await EditKnownMovementAsync (k);
        //    await repo.SaveChangesAsync();
        //   // _ = await PersonalFinanceContext.SaveChangesAsync() > 0;
        //    return Ok(k);
        //}


    }
}
