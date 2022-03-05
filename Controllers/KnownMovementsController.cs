using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PersonalFinance.Models;
using PersonalFinance.Services;

//Known Movements Controller
namespace PersonalFinance.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class KnownMovementsController : PFA_APIController//: PersonalFinanceAPIController
    {

        private readonly IRepository repo;
        public KnownMovementsController(IRepository repo) : base (repo)
        {
            this.repo = repo;
        }

       
        [HttpGet]
        [Route("GetAllKnownMovementsMain")]
        public async Task<IActionResult> KnownMovements_Main(string User_OID)
        {
            KnownMovements_API knownMovements_Main = new KnownMovements_API();
            knownMovements_Main.KnownMovements = await repo.GetAllKnownMovementsAsync();
            knownMovements_Main.KnownMovements = knownMovements_Main.KnownMovements.Where(x => x.Usr_OID == User_OID);
            return Ok(knownMovements_Main);
        }

        //HTTP ADD METHODS
        [HttpPost]
        [Route("AddKnownMovement")]
        public async Task<IActionResult> KnownMovement_Add([FromBody] KnownMovement k)
        {

            await repo.AddKnownMovementAsync(k);
            await repo.SaveChangesAsync();
            return RedirectToAction(nameof(KnownMovements_Main));
        }


        ////HTTP DELETE METHODS

        //[HttpDelete]
        //[Route("DeleteKnownMovement")]
        //public async Task<IActionResult> KnownMovement_Delete(int id)
        //{
        //    var t = await repo.GetKnownMovementAsync(id);
        //    await repo.DeleteKnownMovementAsync(t);
        //    await repo.SaveChangesAsync();
        //    return Ok(t);
        //}


        //HTTP UPDATE METHODS

        [HttpPut]
        [Route("UpdateKnownMovement")]
        public async Task<IActionResult> KnownMovement_Edit(KnownMovement_Ext k)
        {
            await EditKnownMovement(k);
            return Ok(k);
        }

        [HttpPut]
        [Route("UpdateExpOnKnownMovement")]
        public async Task<IActionResult> KnownMovement_Exp_Update(KnownMovement_Exp KM_Exp)
        {
            var KnownMovements = await repo.GetAllKnownMovementsAsync();
            KnownMovements = KnownMovements.Where(x => x.Usr_OID == KM_Exp.Usr_OID);
            foreach (var item in KnownMovements)
            {
                if (item.Exp_ID != 0)
                {
                    if (item.Exp_ID != -1) await ExpToRemoveAsync(item.KMTitle, KM_Exp.Usr_OID, item.Exp_ID);
                    for (int k = 0; k < KM_Exp.Month_Num; k++)
                    {
                        Expiration exp = new Expiration();
                        exp.Usr_OID = item.Usr_OID;
                        exp.ExpTitle = item.KMTitle;
                        exp.ExpDescription = item.KMTitle;
                        exp.ExpDateTime = DateTime.Today.AddMonths(k);
                        exp.ColorLabel = "orange";
                        exp.ExpValue = item.KMValue;
                        await repo.AddExpirationAsync(exp);
                        await repo.SaveChangesAsync();             
                    }
                        var exps = await repo.GetAllExpirationsAsync();
                       IEnumerable<Expiration> Expirations = exps.Where(x => x.Usr_OID == KM_Exp.Usr_OID);
                    item.Exp_ID = Expirations.Last().ID - KM_Exp.Month_Num + 1;
                    await EditKnownMovement((KnownMovement_Ext)item);                  
                } 
            }
            return Ok(1);
        }
    }
}
