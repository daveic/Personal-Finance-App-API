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
        public async Task<IActionResult> AddKnownMovement([FromBody] KnownMovement_Exp k)
        {
            //k.input_value = k.input_value.Replace(".", ",");
            //k.KMValue = Convert.ToDouble(k.input_value);
            if (k.KMValue < 0) k.KMType = "Uscita"; else if (k.KMValue >= 0) k.KMType = "Entrata";
            if (k.On_Exp) k.Exp_ID = -1;
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


        ////HTTP UPDATE METHODS

        //[HttpPut]
        //[Route("UpdateKnownMovement")]
        //public async Task<IActionResult> KnownMovement_Edit(KnownMovement k)
        //{
        //    await repo.UpdateKnownMovementAsync(k);
        //    await repo.SaveChangesAsync();
        //    return Ok(k);
        //}

    }
}
