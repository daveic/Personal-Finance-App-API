using System;
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
    public class PFA_APIController : Controller
    {
        private readonly PersonalFinanceContext PersonalFinanceContext;
        private readonly IRepository repo;
        public PFA_APIController(IRepository repo, PersonalFinanceContext PersonalFinanceContext)
        {
            this.repo = repo;
            this.PersonalFinanceContext = PersonalFinanceContext;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [NonAction]
        public async Task<int> ExpToRemoveAsync(string titleToMatch, string Usr_OID, int ID)
        {
            int maxExp = PersonalFinanceContext.Set<Expiration>().AsNoTracking().AsQueryable().Where(x => x.Usr_OID == Usr_OID).OrderBy(x => x.ID).Last().ID;


            int i = 0;
            bool is_equal = true;
            while (is_equal)
            {
                Expiration e = PersonalFinanceContext.Set<Expiration>().AsNoTracking().AsQueryable().Where(x => x.Usr_OID == Usr_OID).FirstOrDefault(x => x.ID == ID + i);
                if (e != null && e.ExpTitle == titleToMatch) this.PersonalFinanceContext.Remove(e);

                else if (e != null && e.ExpTitle != titleToMatch) is_equal = false;

                else if (ID + i >= maxExp) is_equal = false;

                i++;
            }
            await repo.SaveChangesAsync();
            return 1;
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [NonAction]
        public static string MonthConverter(int monthNum)
        {
            string ConvertedMonth = "";
            switch (monthNum)
            {
                case 1:
                    ConvertedMonth = "Gennaio";
                    break;
                case 2:
                    ConvertedMonth = "Febbraio";
                    break;
                case 3:
                    ConvertedMonth = "Marzo";
                    break;
                case 4:
                    ConvertedMonth = "Aprile";
                    break;
                case 5:
                    ConvertedMonth = "Maggio";
                    break;
                case 6:
                    ConvertedMonth = "Giugno";
                    break;
                case 7:
                    ConvertedMonth = "Luglio";
                    break;
                case 8:
                    ConvertedMonth = "Agosto";
                    break;
                case 9:
                    ConvertedMonth = "Settembre";
                    break;
                case 10:
                    ConvertedMonth = "Ottobre";
                    break;
                case 11:
                    ConvertedMonth = "Novembre";
                    break;
                case 12:
                    ConvertedMonth = "Dicembre";
                    break;
            }
            return ConvertedMonth;
        }
    }
}
