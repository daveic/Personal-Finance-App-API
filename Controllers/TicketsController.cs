using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PersonalFinance.Models;
using PersonalFinance.Services;
using PersonalFinance.Services.EntityFramework;

//Known Movements Controller
namespace PersonalFinance.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class TicketsController : PFA_APIController
    {
        private readonly PersonalFinanceContext PersonalFinanceContext;
        private readonly IRepository repo;
        public TicketsController(IRepository repo, PersonalFinanceContext PersonalFinanceContext) : base(repo)
        {
            this.repo = repo;
            this.PersonalFinanceContext = PersonalFinanceContext;
        }

        [HttpGet]
        [Route("All")]
        public async Task<IActionResult> Tickets_All(string User_OID)
        {
            return Ok(await repo.GetAllTicketsAsync(User_OID));
        }
        [HttpGet]
        [Route("Details")]
        public async Task<IActionResult> Ticket_Details(int id, string User_OID)
        {
            var ticket = await repo.GetTicketAsync(id, User_OID);
            return Ok(ticket);
        }
        [HttpPost]
        [Route("Add")]
        public async Task<IActionResult> Ticket_Add([FromBody] Ticket t)
        {
            var detections = await repo.AddTicketAsync(t);
            await repo.SaveChangesAsync();
            return RedirectToAction(nameof(Tickets_All));
        }
        [HttpPut]
        [Route("Update")]
        public async Task<IActionResult> Ticket_Edit(Ticket t)
        {
            await repo.UpdateTicketAsync(t);
            await repo.SaveChangesAsync();
            return Ok(t);
        }
        [HttpDelete]
        [Route("Delete")]
        public async Task<IActionResult> Ticket_Delete(int id, string User_OID)
        {
            var t = await repo.GetTicketAsync(id, User_OID);
            await repo.DeleteTicketAsync(t);
            await repo.SaveChangesAsync();
            return Ok(t);
        }
    }
}
