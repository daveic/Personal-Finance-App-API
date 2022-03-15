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
        public DebitsController(IRepository repo, PersonalFinanceContext PersonalFinanceContext) : base (repo)
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
            if (d.DebDateTime == DateTime.MinValue)
            {
                d.DebDateTime = d.DebInsDate.AddMonths(Convert.ToInt32((d.RtNum * d.Multiplier)));
            }

            for (int k = 0; k < d.RtNum; k++)
            {
                Expiration exp = new()
                {
                    Usr_OID = d.Usr_OID,
                    ExpTitle = d.DebTitle,
                    ExpDescription = d.DebTitle + "rata: " + (k + 1)
                };
                if (d.RtFreq == "Mesi")
                {
                    exp.ExpDateTime = d.DebInsDate.AddMonths(k * d.Multiplier);
                }
                if (d.RtFreq == "Anni")
                {
                    exp.ExpDateTime = d.DebInsDate.AddYears(k * d.Multiplier);
                }
                exp.ColorLabel = "red";
                exp.ExpValue = d.DebValue / d.RtNum;
                await repo.AddExpirationAsync(exp);
            }
            await repo.SaveChangesAsync();
            d.Exp_ID = PersonalFinanceContext.Set<Expiration>().AsNoTracking().AsQueryable().Where(x => x.Usr_OID == d.Usr_OID).OrderBy(x => x.ID).Last().ID - Convert.ToInt32(d.RtNum) + 1;
            var detections = await repo.AddDebitAsync(d);
            await repo.SaveChangesAsync();
            return RedirectToAction(nameof(Debits_Main));
        }


    }
}
