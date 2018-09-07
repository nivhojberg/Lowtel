using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EF.AspNetCore.Models;
using Lowtel.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Lowtel.Controllers
{
    public class StatisticsController : Controller
    {

        private readonly LotelContext _context;

        public StatisticsController(LotelContext context)
        {
            _context = context;
        }

        // GET: Statistics
        public ActionResult Index()
        {
            if (HttpContext.Session.GetString(UsersController.SessionName) != null || true)
            {
                var starsRate = _context.Hotel.GroupBy(h => h.StarsRate).Select(h => new { starsRate = (h.Key), count = h.Count() }).ToList();
                var starsRateData = JsonConvert.SerializeObject(starsRate);
                ViewBag.starsRateInHotelsData = starsRateData;

                var hotels = _context.Room.GroupBy(r => r.Hotel).Select(i => new { name = ((Hotel)i.Key).Name, roomsAmount = i.Count() });
                var hotelsData = JsonConvert.SerializeObject(hotels);
                ViewBag.roomsAmountInHotelsData = hotelsData;

                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }            
        }
    }
}