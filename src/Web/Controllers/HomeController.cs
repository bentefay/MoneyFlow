using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace spendtracker.Controllers
{
    public class HomeController : ControllerBase
    {
        [HttpGet("/")]
        public ActionResult<string> Get()
        {
            return "Hello, World!";
        }
    }
}
