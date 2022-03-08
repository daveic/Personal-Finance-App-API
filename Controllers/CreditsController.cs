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
            var knownMovement = await repo.GetKnownMovementAsync(id, User_OID);
            if (knownMovement.Exp_ID != 0) knownMovement.On_Exp = true;
            return Ok(knownMovement);
        }

        [HttpGet]
        [Route("Main")]
        public async Task<IActionResult> Credits_Main(string User_OID)
        {            
            return Ok(await repo.GetAllCreditsAsync(User_OID));            
        }

        ////HTTP ADD METHODS
        //[HttpPost]
        //[Route("AddKnownMovement")]
        //public async Task<IActionResult> KnownMovement_Add([FromBody] KnownMovement k)
        //{
        //    if (k.KMValue < 0) k.KMType = "Uscita"; else if (k.KMValue >= 0) k.KMType = "Entrata";
        //    if (k.On_Exp is true) k.Exp_ID = -1;
        //    await repo.AddKnownMovementAsync(k);
        //    await repo.SaveChangesAsync();
        //    return RedirectToAction(nameof(KnownMovements_Main));
        //}


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

        //[HttpPut]
        //[Route("UpdateExpOnKnownMovement")]
        //public async Task<IActionResult> KnownMovement_Exp_UpdateAsync(KnownMovement_Exp KM_Exp)
        //{

        //    var KnownMovements = PersonalFinanceContext.Set<KnownMovement>().AsNoTracking().AsQueryable().Where(x => x.Usr_OID == KM_Exp.Usr_OID).ToList();

        //    // var KnownMovements = repo.GetAllKnownMovementsAsync(KM_Exp.Usr_OID);

        //    foreach (var item in KnownMovements)
        //    {
        //       if (item.Exp_ID != 0)
        //        {
        //            if (item.Exp_ID != -1) await ExpToRemoveAsync(item.KMTitle, KM_Exp.Usr_OID, item.Exp_ID);
        //            for (int k = 0; k < KM_Exp.Month_Num; k++)
        //            {
        //                Expiration exp = new Expiration();
        //                exp.Usr_OID = item.Usr_OID;
        //                exp.ExpTitle = item.KMTitle;
        //                exp.ExpDescription = item.KMTitle;
        //                exp.ExpDateTime = DateTime.Today.AddMonths(k);
        //                exp.ColorLabel = "orange";
        //                exp.ExpValue = item.KMValue;
        //                //this.PersonalFinanceContext.Add(exp);
        //                await repo.AddExpirationAsync(exp);
                       
        //            }
                    
        //            _ = await PersonalFinanceContext.SaveChangesAsync() > 0;
        //          //  await repo.SaveChangesAsync(); 

          
        //            item.Exp_ID = PersonalFinanceContext.Set<Expiration>().AsNoTracking().AsQueryable().Where(x => x.Usr_OID == KM_Exp.Usr_OID).OrderBy(x => x.ID).Last().ID - KM_Exp.Month_Num + 1;

        //            //  item.Exp_ID = Expirations.Last().ID - KM_Exp.Month_Num + 1;
        //            await EditKnownMovementAsync (item);                  
        //        } 
        //    }
        //    _ = await PersonalFinanceContext.SaveChangesAsync() > 0;
        //    return Ok(1);
        //}

        //[ApiExplorerSettings(IgnoreApi = true)]
        //[NonAction]
        //public async Task<int> ExpToRemoveAsync(string titleToMatch, string Usr_OID, int ID)
        //{
        //    int maxExp = PersonalFinanceContext.Set<Expiration>().AsNoTracking().AsQueryable().Where(x => x.Usr_OID == Usr_OID).OrderBy(x => x.ID).Last().ID;


        //    int i = 0;
        //    bool is_equal = true;
        //    while (is_equal)
        //    {
        //        Expiration e = PersonalFinanceContext.Set<Expiration>().AsNoTracking().AsQueryable().Where(x => x.Usr_OID == Usr_OID).FirstOrDefault(x => x.ID == ID + i);
        //        if (e != null && e.ExpTitle == titleToMatch) this.PersonalFinanceContext.Remove(e);
       
        //        else if (e != null && e.ExpTitle != titleToMatch) is_equal = false;
            
        //        else if (ID + i >= maxExp) is_equal = false;
         
        //        i++;
        //    }
        //    await repo.SaveChangesAsync();
        //    return 1;
        //}
        //[ApiExplorerSettings(IgnoreApi = true)]
        //[NonAction]
        //public async Task<KnownMovement> EditKnownMovementAsync(KnownMovement k)
        //{
        //    if (k.KMValue < 0) k.KMType = "Uscita"; else if (k.KMValue >= 0) k.KMType = "Entrata";
        //    if (k.On_Exp is true && k.Exp_ID==0) k.Exp_ID = -1;
        //    if (k.On_Exp is false)
        //    {
        //        string titleToMatch = k.KMTitle;
        //        await ExpToRemoveAsync(titleToMatch, k.Usr_OID, k.Exp_ID);
        //        k.Exp_ID = 0;
        //    }
        //    await repo.UpdateKnownMovementAsync(k);
        //    //PersonalFinanceContext.Attach(k);
        //    //PersonalFinanceContext.Entry(k).State =
        //    //    Microsoft.EntityFrameworkCore.EntityState.Modified;
         
        //    return k;
        //}
    }
}
