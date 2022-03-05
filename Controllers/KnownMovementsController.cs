using System;
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
    public class KnownMovementsController : Controller
    {

        private readonly IRepository repo;
        public KnownMovementsController(IRepository repo)
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
        public async Task<IActionResult> AddKnownMovement([FromBody] KnownMovement k)
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
            var expirations = await repo.GetAllExpirationsAsync();
            int maxExp = expirations.Where(x => x.Usr_OID == k.Usr_OID).Last().ID;
            if (k.KMValue < 0) k.KMType = "Uscita"; else if (k.KMValue >= 0) k.KMType = "Entrata";
            if (k.On_Exp is true) k.Exp_ID = -1;
            if (k.On_Exp is false)
            {
                string titleToMatch = k.KMTitle;
                int i = 0;
                bool is_equal = true;
                while (is_equal)
                {
                    Expiration e = await repo.GetExpirationAsync(k.Exp_ID + i);
                    if (e != null && e.ExpTitle == titleToMatch)
                    {
                        var t = await repo.GetExpirationAsync(e.ID);
                        await repo.DeleteExpirationAsync(t);
                        await repo.SaveChangesAsync();
                    }
                    else if (e != null && e.ExpTitle != titleToMatch)
                    {
                        is_equal = false;
                    }
                    else if (k.Exp_ID + i >= maxExp)
                    {
                        is_equal = false;
                    }
                    i++;
                }
                k.Exp_ID = 0;
            }
            await repo.UpdateKnownMovementAsync(k);
            await repo.SaveChangesAsync();
            return Ok(k);
        }
    }
}
