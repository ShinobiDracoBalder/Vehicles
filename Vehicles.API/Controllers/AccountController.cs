using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vehicles.API.Data;

namespace Vehicles.API.Controllers
{
    public class AccountController : Controller
    {
        private readonly DataContext _dataContext;

        public AccountController(DataContext context)
        {
            _dataContext = context;
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
