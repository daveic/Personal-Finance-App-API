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
        public async Task<IActionResult> Expirations_Main(string User_OID)
        {
            Expirations expirations = new();
            expirations.ExpirationList = await repo.GetAllExpirationsAsync(User_OID);     
            //Trovo gli anni "unici"
            var UniqueYear = expirations.ExpirationList.GroupBy(item => item.ExpDateTime.Year)
                    .Select(group => group.First())
                    .Select(item => item.ExpDateTime.Year)
                    .ToList()
                    .Skip(1);
            List<SelectListItem> itemlistYear = new();
            foreach (var year in UniqueYear.Skip(1)) itemlistYear.Add(new SelectListItem() { Text = year.ToString(), Value = year.ToString() });
            
            //foreach (var year in UniqueYear) expirations.ItemlistYear.Add(new SelectListItem() { Text = year.ToString(), Value = year.ToString() });
expirations.ItemlistYear = itemlistYear;
            expirations.UniqueMonth = expirations.ExpirationList.GroupBy(item => item.ExpDateTime.Month)
                                            .Select(group => group.First())
                                            .Select(item => item.ExpDateTime.Month)
                                            .ToList();            
            return Ok(expirations);
        }

        [HttpPost]
        [Route("Add")]
        public async Task<IActionResult> AddExpiration([FromBody] Expiration e)
        {
            var detections = await repo.AddExpirationAsync(e);
            await repo.SaveChangesAsync();
            return RedirectToAction(nameof(Expirations_Main));
        }


    }
}
