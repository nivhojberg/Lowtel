using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EF.AspNetCore.Models;
using Lowtel.Models;
using Microsoft.AspNetCore.Http;

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
        // This function returns the view of hotels
        // param: searchString - for searching a hotel 
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
        // This function returns the view of details for hotel
        // param: id - hotel id
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
        // This function returns the view of create page
        public IActionResult Create()
        {
            return View();
        }

        // POST: Hotels/Create
        // This function add a new hotel to db
        // param: Hotel - new hotel to create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,State,City,Address,StarsRate,Description,CordX,CordY")] Hotel hotel)
        {
            if (HttpContext.Session.GetString(UsersController.SessionName) != null)
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
            else
            {
                return RedirectToAction("Index", "Home");
            }            
        }

        // This function returns the view of edit for hotel
        // param: id - hotel id
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
        // This function update hotel by id in the db
        // param: id    - hotel id
        //        Hotel - hotel with new values 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,State,City,Address,StarsRate,Description,CordX,CordY")] Hotel hotel)
        {
            if (HttpContext.Session.GetString(UsersController.SessionName) != null)
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
            else
            {
                return RedirectToAction("Index", "Home");
            }        
        }

        // GET: Hotels/Delete/5
        // This function returns the view of delete for hotel
        // param: id - hotel id
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            bool isHotelOnReservation =
                (_context.Reservation.Where(r => r.HotelId == id).Count() > 0);

            var hotel = await _context.Hotel
                .FirstOrDefaultAsync(m => m.Id == id);

            if (hotel == null)
            {
                return NotFound();
            }
            else if (isHotelOnReservation)
            {
                return BadRequest("This hotel is in use on reservation");
            }

            return View(hotel);            
        }

        // POST: Hotels/Delete/5
        // This function deleting hotel by id from the db.
        // param: id - hotel id
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (HttpContext.Session.GetString(UsersController.SessionName) != null)
            {
                var hotel = await _context.Hotel.FindAsync(id);
                _context.Hotel.Remove(hotel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }            
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

        // Return hotel city by cordinates from DB.
        public string GetHotelCityNameByCords()
        {
            double x = Double.Parse(Request.Query["lat"].ToString());
            double y = Double.Parse(Request.Query["lon"].ToString());

            return _context.Hotel.Select(h => new { h.City, h.CordX, h.CordY }).ToList()
                .Where(h => h.CordX == x && h.CordY == y).FirstOrDefault().City;
        }

        // This function returns a list of hotel state from the db.
        public List<string> GetAllHotelsState()
        {
            return _context.Hotel.Select(h => h.State).ToList();
        }
       
        // This function returns a list of hotels cities by state.
        // param: state - state for filter
        public List<string> GetAllHotelsCityByState(string state)
        {
            return _context.Hotel.Where(h => h.State == state).Select(h => h.City).ToList();
        }

        // This function returns the Index view of hotels with a list of hotels by filters
        // param : hotelState   - filter
        //         hotelCity    - filter
        //         minStarsRate - filter
        public async Task<IActionResult> MultiSearch(string hotelState, string hotelCity, int minStarsRate)
        {
            var hotels = _context.Hotel.Where(h => h.State == hotelState &&
            h.City == hotelCity &&
            h.StarsRate >= minStarsRate);

            return View("Index", await hotels.ToListAsync());
        }
    }
}
