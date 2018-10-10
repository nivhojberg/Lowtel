using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EF.AspNetCore.Models;
using Lowtel.Models;

namespace Lowtel.Controllers
{   
    public class HotelsController : Controller
    {
        private readonly LotelContext _context;

        public HotelsController(LotelContext context)
        {
            _context = context;
        }

        // GET: Hotels
        public async Task<IActionResult> Index(string searchString)
        {
            var hotels = from m in _context.Hotel
                         select m;

            if (!String.IsNullOrEmpty(searchString))
            {
                int numberSearch;

                hotels = hotels.Where(h =>
                h.Name.Contains(searchString) ||
                h.State.Contains(searchString) ||
                h.City.Contains(searchString) ||
                h.Address.Contains(searchString) ||
                h.Description.Contains(searchString) ||
                (Int32.TryParse(searchString, out numberSearch) && h.StarsRate == numberSearch));               
            }

            return View(await hotels.ToListAsync());            
        }

        // GET: Hotels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hotel = await _context.Hotel
                .FirstOrDefaultAsync(m => m.Id == id);
            if (hotel == null)
            {
                return NotFound();
            }

            return View(hotel);
        }

        // GET: Hotels/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Hotels/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,State,Address,StarsRate,Description,CordX,CordY")] Hotel hotel)
        {
            // Get hotel last seq id.
            hotel.Id = this.GetLastHotelIdSeq() + 1;

            if (ModelState.IsValid)
            {
                _context.Add(hotel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(hotel);
        }

        // GET: Hotels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hotel = await _context.Hotel.FindAsync(id);
            if (hotel == null)
            {
                return NotFound();
            }
            return View(hotel);
        }

        // POST: Hotels/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,State,Address,StarsRate,Description,CordX,CordY")] Hotel hotel)
        {
            if (id != hotel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(hotel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HotelExists(hotel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(hotel);
        }

        // GET: Hotels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hotel = await _context.Hotel
                .FirstOrDefaultAsync(m => m.Id == id);
            if (hotel == null)
            {
                return NotFound();
            }

            return View(hotel);
        }

        // POST: Hotels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var hotel = await _context.Hotel.FindAsync(id);
            _context.Hotel.Remove(hotel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HotelExists(int id)
        {
            return _context.Hotel.Any(e => e.Id == id);
        }

        // Select map cordinates of all the hotels.
        public dynamic GetHotelsCords()
        {
            return _context.Hotel.Select(h => new { h.Name, h.CordX, h.CordY }).ToList();
        }

        private int GetLastHotelIdSeq()
        {
            return _context.Hotel.Select(h => h.Id).Max();
        }

        [HttpGet]
        public string GetHotelCityNameByCords()
        {
            double x = Double.Parse(Request.Query["lat"].ToString());
            double y = Double.Parse(Request.Query["lon"].ToString());
            string address = _context.Hotel.Select(h => new { h.Address, h.CordX, h.CordY }).ToList()
                .Where(h => h.CordX > x - 5 && h.CordX < x + 5 && h.CordY > y - 5 && h.CordY < y + 5).FirstOrDefault().Address;
            return address.Substring(0, address.IndexOf(','));
        }

        public List<string> GetAllHotelsState()
        {
            return _context.Hotel.Select(h => h.State).ToList();
        }
       
        public List<string> GetAllHotelsCityByState(string state)
        {
            return _context.Hotel.Where(h => h.State == state).Select(h => h.City).ToList();
        }

        public async Task<IActionResult> MultiSearch(string hotelState, string hotelCity, int minStarsRate)
        {
            var hotels = _context.Hotel.Where(h => h.State == hotelState &&
            h.City == hotelCity &&
            h.StarsRate >= minStarsRate);

            return View("Index", await hotels.ToListAsync());
        }
    }
}
