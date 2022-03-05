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
    public class PFA_APIController : Controller//: PersonalFinanceAPIController
    {

        private readonly IRepository repo;
        public PFA_APIController(IRepository repo)
        {
            this.repo = repo;
        }
        [HttpPost]
        public async Task ExpToRemoveAsync(string titleToMatch, string Usr_OID, int ID)
        {
            var expirations = await repo.GetAllExpirationsAsync();
            int maxExp = expirations.Where(x => x.Usr_OID == Usr_OID).OrderBy(x => x.ID).Last().ID;
            int i = 0;
            bool is_equal = true;
            while (is_equal)
            {
                Expiration e = await repo.GetExpirationAsync(ID + i);
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
                else if (ID + i >= maxExp)
                {
                    is_equal = false;
                }
                i++;
            }
        }
        [HttpPost]
        public async Task<KnownMovement> EditKnownMovement(KnownMovement_Ext k)
        {
            if (k.KMValue < 0) k.KMType = "Uscita"; else if (k.KMValue >= 0) k.KMType = "Entrata";
            if (k.On_Exp is true) k.Exp_ID = -1;
            if (k.On_Exp is false)
            {
                string titleToMatch = k.KMTitle;
                await ExpToRemoveAsync(titleToMatch, k.Usr_OID, k.Exp_ID);
                k.Exp_ID = 0;
            }
            await repo.UpdateKnownMovementAsync(k);
            await repo.SaveChangesAsync();
            return k;
        }
    }
}
