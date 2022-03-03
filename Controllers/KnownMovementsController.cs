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
    public class KnownMovementsController : ControllerBase
    {

        private readonly IRepository repo;
        public KnownMovementsController(IRepository repo)
        {
            this.repo = repo;
        }

        [Route("GetAllKnownMovementsMain")]
        public async Task<IActionResult> KnownMovements_Main(string User_OID)
        {
            KnownMovements_API knownMovements_Main = new KnownMovements_API();
            knownMovements_Main.KnownMovements = await repo.GetAllKnownMovementsAsync();
            knownMovements_Main.KnownMovements = knownMovements_Main.KnownMovements.Where(x => x.Usr_OID == User_OID);
            knownMovements_Main.Expirations = (System.Collections.Generic.List<Expiration>)await repo.GetAllExpirationsAsync();
            knownMovements_Main.Expirations = knownMovements_Main.Expirations.OrderBy(x => x.ExpDateTime.Month).Take(5).ToList(); //Fetch imminent expirations
            knownMovements_Main.KnownMovement = new KnownMovement();
            return Ok(knownMovements_Main);
        }

        //HTTP GET ALL METHODS

        [Route("GetAllKnownMovements")]
        public async Task<IActionResult> AllKnownMovements(string User_OID)
        {
            var knownMovements = await repo.GetAllKnownMovementsAsync();
            knownMovements = knownMovements.Where(x => x.Usr_OID == User_OID);
            return Ok(knownMovements);
        }


        //HTTP GET BY ID METHODS

        [HttpGet]
        [Route("GetKnownMovementId")]
        public async Task<IActionResult> KnownMovement_Details(int id)
        {
            var knownMovement = await repo.GetKnownMovementAsync(id);
            return Ok(knownMovement);
        }


        //HTTP ADD METHODS

        [HttpPost]
        [Route("AddKnownMovement")]
        public async Task<IActionResult> AddKnownMovement([FromBody] KnownMovement k)
        {
            var detections = await repo.AddKnownMovementAsync(k);
            await repo.SaveChangesAsync();
            return RedirectToAction(nameof(AllKnownMovements));
        }


        //HTTP DELETE METHODS

        [HttpDelete]
        [Route("DeleteKnownMovement")]
        public async Task<IActionResult> KnownMovement_Delete(int id)
        {
            var t = await repo.GetKnownMovementAsync(id);
            await repo.DeleteKnownMovementAsync(t);
            await repo.SaveChangesAsync();
            return Ok(t);
        }


        //HTTP UPDATE METHODS

        [HttpPut]
        [Route("UpdateKnownMovement")]
        public async Task<IActionResult> KnownMovement_Edit(KnownMovement k)
        {
            await repo.UpdateKnownMovementAsync(k);
            await repo.SaveChangesAsync();
            return Ok(k);
        }
        
    }
}
