﻿using System;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    public class HomeController : ControllerBase
    {
        [HttpGet("/")]
        public ActionResult<string> Get()
        {
            return $"Hello, World! It's {DateTime.Now.DayOfWeek}.";
        }
    }
}
