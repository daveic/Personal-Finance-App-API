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
        
    }
}
