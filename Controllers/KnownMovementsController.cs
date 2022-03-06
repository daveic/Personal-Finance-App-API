using System;
using System.Collections;
using System.Collections.Generic;
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
    public class KnownMovementsController : PFA_APIController//: PersonalFinanceAPIController
    {

        private readonly PersonalFinanceContext PersonalFinanceContext;
        private readonly IRepository repo;
        public KnownMovementsController(IRepository repo, PersonalFinanceContext PersonalFinanceContext) : base (repo)
        {
            this.repo = repo;
            this.PersonalFinanceContext = PersonalFinanceContext;
        }
        


        [HttpGet]
        [Route("GetAllKnownMovementsMain")]
        public async Task<IActionResult> KnownMovements_Main(string User_OID)
        {
            KnownMovements_API knownMovements_Main = new KnownMovements_API();
            knownMovements_Main.KnownMovements = await repo.GetAllKnownMovementsAsync(User_OID);
            //knownMovements_Main.KnownMovements = knownMovements_Main.KnownMovements.Where(x => x.Usr_OID == User_OID);
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

            var KnownMovements = PersonalFinanceContext.Set<KnownMovement>().AsNoTracking().AsQueryable().Where(x => x.Usr_OID == KM_Exp.Usr_OID);
            //KnownMovements = KnownMovements.Where(x => x.Usr_OID == KM_Exp.Usr_OID);
            // var KnownMovements = repo.GetAllKnownMovementsAsync(KM_Exp.Usr_OID);

         
            //KnownMovements = (IQueryable<KnownMovement>)KnownMovements.ToList();
            //KnownMovements = (IQueryable<KnownMovement>)KnownMovements.Where(x => x.Usr_OID == KM_Exp.Usr_OID);
            foreach (var item in KnownMovements)
            {
               if (item.Exp_ID != 0)
                {
                    if (item.Exp_ID != -1) ExpToRemove(item.KMTitle, KM_Exp.Usr_OID, item.Exp_ID);
                    for (int k = 0; k < KM_Exp.Month_Num; k++)
                    {
                        Expiration exp = new Expiration();
                        exp.Usr_OID = item.Usr_OID;
                        exp.ExpTitle = item.KMTitle;
                        exp.ExpDescription = item.KMTitle;
                        exp.ExpDateTime = DateTime.Today.AddMonths(k);
                        exp.ColorLabel = "orange";
                        exp.ExpValue = item.KMValue;
                        this.PersonalFinanceContext.Add(exp);
                        //await repo.AddExpirationAsync(exp);
                                    
                    }
                       await repo.SaveChangesAsync(); 

                      // var exps = await repo.GetAllExpirationsAsync();
                      // IEnumerable<Expiration> Expirations = exps.Where(x => x.Usr_OID == KM_Exp.Usr_OID).ToList();
                    item.Exp_ID = PersonalFinanceContext.Set<Expiration>().AsNoTracking().AsQueryable().Where(x => x.Usr_OID == KM_Exp.Usr_OID).OrderBy(x => x.ID).Last().ID - KM_Exp.Month_Num + 1;

                  //  item.Exp_ID = Expirations.Last().ID - KM_Exp.Month_Num + 1;
                    await EditKnownMovement((KnownMovement_Ext)item);                  
                } 
            }
            return Ok(1);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [NonAction]
        public int ExpToRemove(string titleToMatch, string Usr_OID, int ID)
        {
            int maxExp = PersonalFinanceContext.Set<Expiration>().AsNoTracking().AsQueryable().Where(x => x.Usr_OID == Usr_OID).OrderBy(x => x.ID).Last().ID;

            //var expirations = await repo.GetAllExpirationsAsync();
            ////expirations = (IQueryable<Expiration>)expirations.ToList();
            //int maxExp = expirations.Where(x => x.Usr_OID == Usr_OID).OrderBy(x => x.ID).Last().ID;
            int i = 0;
            bool is_equal = true;
            while (is_equal)
            {
                Expiration e = PersonalFinanceContext.Set<Expiration>().AsNoTracking().AsQueryable().Where(x => x.Usr_OID == Usr_OID).FirstOrDefault(x => x.ID == ID);
                if (e != null && e.ExpTitle == titleToMatch)
                {
                    //var t = await repo.GetExpirationAsync(e.ID);
                    this.PersonalFinanceContext.Remove(e);
                    //await repo.DeleteExpirationAsync(e);
                    
                }
                else if (e != null && e.ExpTitle != titleToMatch)
                {
                    is_equal = false;
                }
                else if (ID + i >= maxExp)
                {
                    is_equal = false;
                }
                i++;
            }
            repo.SaveChangesAsync();
            return 1;
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [NonAction]
        public async Task<KnownMovement> EditKnownMovement(KnownMovement_Ext k)
        {
            if (k.KMValue < 0) k.KMType = "Uscita"; else if (k.KMValue >= 0) k.KMType = "Entrata";
            if (k.On_Exp is true) k.Exp_ID = -1;
            if (k.On_Exp is false)
            {
                string titleToMatch = k.KMTitle;
                ExpToRemove(titleToMatch, k.Usr_OID, k.Exp_ID);
                k.Exp_ID = 0;
            }
            await repo.UpdateKnownMovementAsync(k);
            await repo.SaveChangesAsync();
            return k;
        }
    }
}
