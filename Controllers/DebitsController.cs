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
    public class DebitsController : PFA_APIController
    {
        private readonly PersonalFinanceContext PersonalFinanceContext;
        private readonly IRepository repo;
        public DebitsController(IRepository repo, PersonalFinanceContext PersonalFinanceContext) : base (repo, PersonalFinanceContext)
        {
            this.repo = repo;
            this.PersonalFinanceContext = PersonalFinanceContext;
        }

        [HttpGet]
        [Route("All")]
        public async Task<IActionResult> Debits_Main(string User_OID)
        {
            return Ok(await repo.GetAllDebitsAsync(User_OID));
        }
        [HttpGet]
        [Route("Details")]
        public async Task<IActionResult> Debit_Details(int id, string User_OID)
        {
            return Ok(await repo.GetDebitAsync(id, User_OID));
        }
        [HttpPost]
        [Route("Add")]
        public async Task<IActionResult> Debit_Add([FromBody] Debit d)
        {
            await Debit_Add_Service(d);
            return RedirectToAction(nameof(Debits_Main));
        }
        

        [HttpPut]
        [Route("Update")]
        public async Task<IActionResult> Debits_Edit(Debit d)
        {
            Debit oldDebit = PersonalFinanceContext.Set<Debit>().AsNoTracking().AsQueryable().Where(x => x.Usr_OID == d.Usr_OID).FirstOrDefault(x => x.ID == d.ID);
            int i = await ExpToRemoveAsync(d.DebCode, d.Usr_OID, d.Exp_ID);
            for (int j = 0; j < (d.RtNum - d.RtPaid); j++)
            {
                Expiration exp = new()
                {
                    Usr_OID = d.Usr_OID,
                    ExpTitle = d.DebCode,
                };
                if (d.Multiplier != 0)
                {
                    if (d.RtFreq == "Mesi")
                    {
                        exp.ExpDateTime = d.DebInsDate.AddMonths(j * d.Multiplier);
                    }
                    if (d.RtFreq == "Anni")
                    {
                        exp.ExpDateTime = d.DebInsDate.AddYears(j * d.Multiplier);
                    }
                    exp.ExpDescription = d.DebTitle + " - rata: " + (j + 1);
                    exp.ExpValue = d.RemainToPay / (d.RtNum - d.RtPaid);
                } else
                {
                    exp.ExpDateTime = d.DebDateTime;
                    exp.ExpDescription = d.DebTitle;
                    exp.ExpValue = d.DebValue / (d.RtNum - d.RtPaid);
                }

                exp.ColorLabel = "red";
                
                await repo.AddExpirationAsync(exp);
                await repo.SaveChangesAsync();
            }
            d.Exp_ID = PersonalFinanceContext.Set<Expiration>().AsNoTracking().AsQueryable().Where(x => x.Usr_OID == d.Usr_OID).OrderBy(x => x.ID).Last().ID - Convert.ToInt32(d.RtNum) + 1;
            await repo.UpdateDebitAsync(d);
            await repo.SaveChangesAsync();
            return Ok(d);
        }
        [HttpPut]
        [Route("UpdateOnTransaction")]
        public async Task<IActionResult> Debits_Edit_OnTransaction(Debit d)
        {
            await repo.UpdateDebitAsync(d);
            await repo.SaveChangesAsync();
            return Ok(d);
        }
        [HttpDelete]
        [Route("Delete")]
        public async Task<IActionResult> Debit_Delete(int id, string User_OID)
        {
            Debit deb = PersonalFinanceContext.Set<Debit>().AsNoTracking().AsQueryable().Where(x => x.Usr_OID == User_OID).FirstOrDefault(x => x.ID == id);
            await ExpToRemoveAsync(deb.DebCode, User_OID, deb.Exp_ID);
            //for (int i = 0; i <= (deb.RtNum - deb.RtPaid); i++)
            //{
            //    int res = DeleteItemN("Expirations", (deb.Exp_ID + i), GetUserData().Result);
            //}
            var t = await repo.GetDebitAsync(id, User_OID);
            await repo.DeleteDebitAsync(t);
            await repo.SaveChangesAsync();
            return Ok(t);
        }
    }
}
