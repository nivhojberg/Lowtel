using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Lowtel.Models;
using Microsoft.AspNetCore.Http;

namespace Lowtel.Controllers
{
    public class HomeController : Controller
    {
        // This function returns the home page
        public IActionResult Index()
        {
            //HttpContext.Session.SetString(UsersController.SessionName, "admin");
            return View();
        }

        // This function returns the view of about page
        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        // This function returns the view of contact page
        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
