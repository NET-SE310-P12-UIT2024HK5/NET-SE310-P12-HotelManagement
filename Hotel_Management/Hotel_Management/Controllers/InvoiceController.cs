﻿using Microsoft.AspNetCore.Mvc;

namespace Hotel_Management.Controllers
{
    public class InvoiceController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
