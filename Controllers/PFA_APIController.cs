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
        public async Task<int> Credit_Add_Service(Credit c)
        {
            Expiration exp = new()
            {
                Usr_OID = c.Usr_OID,
                ExpTitle = c.CredCode,
                ExpDescription = "Rientro previsto - " + c.CredTitle,
                ExpDateTime = c.PrevDateTime,
                ColorLabel = "green",
                ExpValue = c.CredValue
            };
            await repo.AddExpirationAsync(exp);
            await repo.SaveChangesAsync();
            c.Exp_ID = PersonalFinanceContext.Set<Expiration>().AsNoTracking().AsQueryable().Where(x => x.Usr_OID == c.Usr_OID).OrderBy(x => x.ID).Last().ID;
            await repo.AddCreditAsync(c);
            await repo.SaveChangesAsync();
            return 1;
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [NonAction]
        public async Task<Credit> Credit_Edit_Service(Credit c)
        {
            var Expirations = PersonalFinanceContext.Set<Expiration>().AsNoTracking().AsQueryable().Where(x => x.Usr_OID == c.Usr_OID).ToList();
            foreach (var exp in Expirations)
            {
                if (c.Exp_ID == exp.ID)
                {
                    this.PersonalFinanceContext.Remove(exp);
                    _ = PersonalFinanceContext.SaveChanges() > 0;
                    Expiration e = new()
                    {
                        Usr_OID = c.Usr_OID,
                        ExpTitle = c.CredCode,
                        ExpDescription = "Rientro previsto - " + c.CredTitle,
                        ExpDateTime = c.PrevDateTime,
                        ColorLabel = "green",
                        ExpValue = c.CredValue
                    };
                    this.PersonalFinanceContext.Add(e);
                    _ = PersonalFinanceContext.SaveChanges() > 0;
                    break;
                }
            }
            c.Exp_ID = PersonalFinanceContext.Set<Expiration>().AsNoTracking().AsQueryable().Where(x => x.Usr_OID == c.Usr_OID).OrderBy(x => x.ID).Last().ID;
            await repo.UpdateCreditAsync(c);
            await repo.SaveChangesAsync();
            return c;
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [NonAction]
        public async Task<int> Debit_Add_Service(Debit d)
        {
            if (d.DebDateTime == DateTime.MinValue)
            {
                d.DebDateTime = d.DebInsDate.AddMonths(Convert.ToInt32((d.RtNum * d.Multiplier)));
            }
            if (d.Multiplier == 0)
            {
                Expiration exp = new()
                {
                    Usr_OID = d.Usr_OID,
                    ExpTitle = d.DebCode,
                    ExpDescription = d.DebTitle,
                    ExpDateTime = d.DebDateTime,
                    ColorLabel = "red",
                    ExpValue = d.DebValue
                };
                await repo.AddExpirationAsync(exp);
                await repo.SaveChangesAsync();
                d.Exp_ID = PersonalFinanceContext.Set<Expiration>().AsNoTracking().AsQueryable().Where(x => x.Usr_OID == d.Usr_OID).OrderBy(x => x.ID).Last().ID;
            }
            else
            {
                for (int k = 0; k < d.RtNum; k++)
                {
                    Expiration exp = new()
                    {
                        Usr_OID = d.Usr_OID,
                        ExpTitle = d.DebCode,
                        ExpDescription = d.DebTitle + "rata: " + (k + 1)
                    };
                    if (d.RtFreq == "Mesi")
                    {
                        exp.ExpDateTime = d.DebInsDate.AddMonths(k * d.Multiplier);
                        d.DebDateTime = d.DebInsDate.AddMonths(Convert.ToInt32((d.RtNum * d.Multiplier)));
                    }
                    if (d.RtFreq == "Anni")
                    {
                        exp.ExpDateTime = d.DebInsDate.AddYears(k * d.Multiplier);
                        d.DebDateTime = d.DebInsDate.AddYears(Convert.ToInt32((d.RtNum * d.Multiplier)));
                    }
                    exp.ColorLabel = "red";
                    exp.ExpValue = d.DebValue / d.RtNum;
                    await repo.AddExpirationAsync(exp);
                }
                await repo.SaveChangesAsync();
                d.Exp_ID = PersonalFinanceContext.Set<Expiration>().AsNoTracking().AsQueryable().Where(x => x.Usr_OID == d.Usr_OID).OrderBy(x => x.ID).Last().ID - Convert.ToInt32(d.RtNum) + 1;
            }
            d.Hide = 0;
            var detections = await repo.AddDebitAsync(d);
            await repo.SaveChangesAsync();
            return 1;
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [NonAction]
        public async Task<int> Debit_Edit_Service(Debit d)
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
                }
                else
                {
                    exp.ExpDateTime = d.DebDateTime;
                    exp.ExpDescription = d.DebTitle;
                    exp.ExpValue = d.RemainToPay;
                }

                exp.ColorLabel = "red";
                await repo.AddExpirationAsync(exp);
                await repo.SaveChangesAsync();
            }
            d.Exp_ID = PersonalFinanceContext.Set<Expiration>().AsNoTracking().AsQueryable().Where(x => x.Usr_OID == d.Usr_OID).OrderBy(x => x.ID).Last().ID - Convert.ToInt32(d.RtNum) + 1;
            await repo.UpdateDebitAsync(d);
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
