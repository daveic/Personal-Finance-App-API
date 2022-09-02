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
    public class KnownMovementsController : PFA_APIController
    {
        private readonly PersonalFinanceContext PersonalFinanceContext;
        private readonly IRepository repo;
        public KnownMovementsController(IRepository repo, PersonalFinanceContext PersonalFinanceContext) : base (repo, PersonalFinanceContext)
        {
            this.repo = repo;
            this.PersonalFinanceContext = PersonalFinanceContext;
        }

        [HttpGet]
        [Route("All")]
        public async Task<IActionResult> KnownMovements_Main(string User_OID)
        {
            return Ok(await repo.GetAllKnownMovementsAsync(User_OID));
        }
        [HttpGet]
        [Route("Details")]
        public async Task<IActionResult> KnownMovement_Details(int id, string User_OID)
        {
            var knownMovement = await repo.GetKnownMovementAsync(id, User_OID);
            if (knownMovement.Exp_ID != 0) knownMovement.On_Exp = true;
            return Ok(knownMovement);
        }
        [HttpPost]
        [Route("Add")]
        public async Task<IActionResult> KnownMovement_Add([FromBody] KnownMovement k)
        {
            if (k.KMValue < 0) k.KMType = "Uscita"; else if (k.KMValue >= 0) k.KMType = "Entrata";
            if (k.On_Exp is true) k.Exp_ID = -1;
            await repo.AddKnownMovementAsync(k);
            await repo.SaveChangesAsync();
            return RedirectToAction(nameof(KnownMovements_Main));
        }
        [HttpPut]
        [Route("Update")]
        public async Task<IActionResult> KnownMovement_EditAsync(KnownMovement k)
        {
            await EditKnownMovementAsync (k);
            await repo.SaveChangesAsync();
           // _ = await PersonalFinanceContext.SaveChangesAsync() > 0;
            return Ok(k);
        }
        [HttpDelete]
        [Route("Delete")]
        public async Task<IActionResult> KnownMovement_Delete(int id, string User_OID)
        {
            var km = await repo.GetKnownMovementAsync(id, User_OID);
            await ExpToRemoveAsync(km.KMTitle, km.Usr_OID, km.Exp_ID);
            await repo.DeleteKnownMovementAsync(km);
            await repo.SaveChangesAsync();
            return Ok(km);
        }






        [HttpPut]
        [Route("UpdateExpOnKnownMovement")]
        public async Task<IActionResult> KnownMovement_Exp_UpdateAsync(KnownMovement_Exp KM_Exp)
        {

            var KnownMovements = PersonalFinanceContext.Set<KnownMovement>().AsNoTracking().AsQueryable().Where(x => x.Usr_OID == KM_Exp.Usr_OID).ToList();

            // var KnownMovements = repo.GetAllKnownMovementsAsync(KM_Exp.Usr_OID);

            foreach (var item in KnownMovements)
            {
               if (item.Exp_ID != 0)
                {
                    if (item.Exp_ID != -1) await ExpToRemoveAsync(item.KMTitle, KM_Exp.Usr_OID, item.Exp_ID);
                    for (int k = 0; k < KM_Exp.Month_Num; k++)
                    {
                        Expiration exp = new()
                        {
                            Usr_OID = item.Usr_OID,
                            ExpTitle = item.KMTitle,
                            ExpDescription = item.KMTitle,
                            ExpDateTime = DateTime.Today.AddMonths(k),
                            ColorLabel = "orange",
                            ExpValue = item.KMValue
                        };
                        //this.PersonalFinanceContext.Add(exp);
                        await repo.AddExpirationAsync(exp);
                       
                    }
                    
                    _ = await PersonalFinanceContext.SaveChangesAsync() > 0;
                  //  await repo.SaveChangesAsync(); 

          
                    item.Exp_ID = PersonalFinanceContext.Set<Expiration>().AsNoTracking().AsQueryable().Where(x => x.Usr_OID == KM_Exp.Usr_OID).OrderBy(x => x.ID).Last().ID - KM_Exp.Month_Num + 1;

                    //  item.Exp_ID = Expirations.Last().ID - KM_Exp.Month_Num + 1;
                    await EditKnownMovementAsync (item);                  
                } 
            }
            _ = await PersonalFinanceContext.SaveChangesAsync() > 0;
            return Ok(1);
        }

        
        [ApiExplorerSettings(IgnoreApi = true)]
        [NonAction]
        public async Task<KnownMovement> EditKnownMovementAsync(KnownMovement k)
        {
            if (k.KMValue < 0) k.KMType = "Uscita"; else if (k.KMValue >= 0) k.KMType = "Entrata";
            if (k.Exp_ID != 0)
            {
                Expiration exp = PersonalFinanceContext.Set<Expiration>().AsNoTracking().AsQueryable().Where(x => x.Usr_OID == k.Usr_OID).FirstOrDefault(x => x.ID == k.Exp_ID);
                if(exp != null)
                {
                    if(k.KMTitle != exp.ExpTitle || k.KMValue != exp.ExpValue)
                    {
                        int maxExp = PersonalFinanceContext.Set<Expiration>().AsNoTracking().AsQueryable().Where(x => x.Usr_OID == k.Usr_OID).OrderBy(x => x.ID).Last().ID;

                        int i = 0;
                        bool is_equal = true;
                        while (is_equal)
                        {
                            Expiration e = PersonalFinanceContext.Set<Expiration>().AsNoTracking().AsQueryable().Where(x => x.Usr_OID == k.Usr_OID).FirstOrDefault(x => x.ID == k.Exp_ID + i);
                            if (e != null && e.ExpDescription == exp.ExpTitle) 
                            {
                                e.ExpTitle = k.KMTitle;
                                e.ExpValue = k.KMValue;
                                e.ExpDescription = k.KMTitle;
                                PersonalFinanceContext.Attach(e);
                                PersonalFinanceContext.Entry(e).State =
                                    Microsoft.EntityFrameworkCore.EntityState.Modified;
                            
                            }
                            else if (e != null && e.ExpDescription != exp.ExpTitle) is_equal = false;
                            else if (k.Exp_ID + i >= maxExp) is_equal = false;

                            i++;
                        }
                        _ = await PersonalFinanceContext.SaveChangesAsync() > 0;
                    }
                }                
            }

            if (k.On_Exp is true && k.Exp_ID==0) k.Exp_ID = -1;
            if (k.On_Exp is false)
            {
                await ExpToRemoveAsync(k.KMTitle, k.Usr_OID, k.Exp_ID);
                k.Exp_ID = 0;
            }
            await repo.UpdateKnownMovementAsync(k);
         
            return k;
        }
    }
}
