using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PersonalFinance.Models;
using PersonalFinance.Services;
using PersonalFinance.Services.EntityFramework;

//Known Movements Controller
namespace PersonalFinance.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class ExpirationsController : PFA_APIController
    {
        private readonly PersonalFinanceContext PersonalFinanceContext;
        private readonly IRepository repo;
        public ExpirationsController(IRepository repo, PersonalFinanceContext PersonalFinanceContext) : base (repo)
        {
            this.repo = repo;
            this.PersonalFinanceContext = PersonalFinanceContext;
        }

        [HttpGet]
        [Route("All")]
        public async Task<IActionResult> Expirations_Main(string User_OID, string selectedYear)
        {
            Expirations expirations = new();
            expirations.ExpirationList = await repo.GetAllExpirationsAsync(User_OID);
            //Trovo gli anni "unici"
            var expOtherYears = expirations.ExpirationList.Where(x => x.ExpDateTime.Year != DateTime.Now.Year);
            var UniqueYear = expOtherYears
                    .GroupBy(item => item.ExpDateTime.Year)
                    .Select(group => group.First())
                    .Select(item => item.ExpDateTime.Year)
                    .ToList();
            List<SelectListItem> itemlistYear = new();
            foreach (var year in UniqueYear) itemlistYear.Add(new SelectListItem() { Text = year.ToString(), Value = year.ToString() });
            expirations.ItemlistYear = itemlistYear;
            if (!String.IsNullOrEmpty(selectedYear)) expirations.ExpirationList = expirations.ExpirationList.AsQueryable().Where(x => x.ExpDateTime.Year.ToString() == selectedYear).OrderBy(item => item.ExpDateTime.Month);
            else expirations.ExpirationList = expirations.ExpirationList.AsQueryable().Where(x => x.ExpDateTime.Year.ToString() == DateTime.Now.Year.ToString()).OrderBy(item => item.ExpDateTime.Month);

            expirations.UniqueMonth = expirations.ExpirationList.GroupBy(item => item.ExpDateTime.Month)
                                            .Select(group => group.First())
                                            .Select(item => item.ExpDateTime.Month)
                                            .ToList();

            List<string> UniqueMonthNames = new();
            List<ExpMonth> expMonth = new();
            foreach (var month in expirations.UniqueMonth)
            {
                UniqueMonthNames.Add(MonthConverter(month));
                var singleMonthExp = expirations.ExpirationList.AsQueryable().Where(x => x.ExpDateTime.Month.ToString() == month.ToString());
                foreach (var exp in singleMonthExp)
                {
                    ExpMonth item = new();
                    item.Month = MonthConverter(month);
                    item.ExpItem = exp;
                    expMonth.Add(item);
                }
            }
            expirations.UniqueMonthNames = UniqueMonthNames;
            expirations.ExpMonth = expMonth;
            return Ok(expirations);
        }
        [HttpGet]

        [Route("GetFirst")]
        public IActionResult Expirations_First(string User_OID)
        {
            return Ok(PersonalFinanceContext.Set<Expiration>().AsNoTracking().AsQueryable().Where(x => x.Usr_OID == User_OID).OrderBy(x => x.ExpDateTime.Month).Take(5).ToList());
        }
        [HttpPost]
        [Route("Add")]
        public async Task<IActionResult> AddExpiration([FromBody] Expiration e)
        {
            var detections = await repo.AddExpirationAsync(e);
            await repo.SaveChangesAsync();
            return RedirectToAction(nameof(Expirations_Main));
        }
        [HttpGet]
        [Route("Details")]
        public async Task<IActionResult> Expiration_Details(int id, string User_OID)
        {
            var expiration = await repo.GetExpirationAsync(id, User_OID);
            return Ok(expiration);
        }
        [HttpDelete]
        [Route("Delete")]
        public async Task<IActionResult> Expiration_Delete(int id, string User_OID)
        {
            var t = await repo.GetExpirationAsync(id, User_OID);
            await repo.DeleteExpirationAsync(t);
            await repo.SaveChangesAsync();
            return Ok(t);
        }
    }
}
