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
    public class RoomsController : Controller
    {
        private readonly LotelContext _context;

        public RoomsController(LotelContext context)
        {
            _context = context;
        }

        // GET: Rooms
        public async Task<IActionResult> Index(string searchString)
        {

            IQueryable<Room> rooms = _context.Set<Room>();

            rooms = rooms.Include(r => r.Hotel).Include(r => r.RoomType);

            if (!String.IsNullOrEmpty(searchString))
            {
                int numberSearch;

                rooms = rooms.Where(r =>
                r.Hotel.Name.Contains(searchString) ||
                r.Hotel.State.Contains(searchString) ||
                r.RoomType.Name.Contains(searchString)||
                (Int32.TryParse(searchString, out numberSearch) &&
                (r.Id == numberSearch || r.RoomType.PriceForNight == numberSearch)));
            }


            rooms = rooms.OrderBy(r => r.HotelId).ThenBy(r => r.Id);

            return View(await rooms.ToListAsync());
        }

        // GET: Rooms/Details/5
        public async Task<IActionResult> Details(int? id, int? hotelId)
        {
            if (id == null || hotelId == null)
            {
                return NotFound();
            }

            var room = await _context.Room
                .Include(r => r.Hotel)
                .Include(r => r.RoomType)
                .FirstOrDefaultAsync(m => (m.Id == id) && (m.HotelId == hotelId));
            if (room == null)
            {
                return NotFound();
            }

            return View(room);
        }

        // GET: Rooms/Create
        public IActionResult Create()
        {
            ViewData["HotelId"] = new SelectList(_context.Hotel, "Id", "Name");
            ViewData["RoomTypeId"] = new SelectList(_context.RoomType, "Id", "Name");
            return View(new Room());
        }

        // POST: Rooms/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,HotelId,RoomTypeId,IsFree")] Room room)
        {
            if (ModelState.IsValid)
            {
                _context.Add(room);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["HotelId"] = new SelectList(_context.Hotel, "Id", "Id", room.HotelId);
            ViewData["RoomTypeId"] = new SelectList(_context.RoomType, "Id", "Id", room.RoomTypeId);
            return View(room);
        }

        // GET: Rooms/Edit/5
        public async Task<IActionResult> Edit(int? id, int? hotelId)
        {
            if (id == null || hotelId == null)
            {
                return NotFound();
            }

            var room = await _context.Room.FindAsync(id, hotelId);
            if (room == null)
            {
                return NotFound();
            }
            ViewData["HotelId"] = new SelectList(_context.Hotel, "Id", "Id", room.HotelId);
            ViewData["RoomTypeId"] = new SelectList(_context.RoomType, "Id", "Name", room.RoomTypeId);
            return View(room);
        }

        // POST: Rooms/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,HotelId,RoomTypeId,IsFree")] Room room)
        {
            if (id != room.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(room);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RoomExists(room.Id, room.HotelId))
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
            ViewData["HotelId"] = new SelectList(_context.Hotel, "Id", "Id", room.HotelId);
            ViewData["RoomTypeId"] = new SelectList(_context.RoomType, "Id", "Id", room.RoomTypeId);
            return View(room);
        }

        // GET: Rooms/Delete/5
        public async Task<IActionResult> Delete(int? id, int? hotelId)
        {
            if (id == null || hotelId == null)
            {
                return NotFound();
            }

            var room = await _context.Room
                .Include(r => r.Hotel)
                .Include(r => r.RoomType)
                .FirstOrDefaultAsync(m => (m.Id == id) && (m.HotelId == hotelId));
            if (room == null)
            {
                return NotFound();
            }

            return View(room);
        }

        // POST: Rooms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, int hotelId)
        {
            var room = await _context.Room.FindAsync(id, hotelId);
            _context.Room.Remove(room);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RoomExists(int id, int hotelId)
        {
            return _context.Room.Any(e => (e.Id == id) && (e.HotelId == hotelId));
        }

        public int GetLastRoomNumberInHotel(int hotelId)
        {
            var query = _context.Room.Where(room => (room.HotelId == hotelId));

            if (query.Any())
            {
                return query.Max(room => room.Id);
            }
            else
            {
                return 0;
            }
        }

        public dynamic GetFreeRoomByParms(int hotelId,int roomTypeId)
        {          
            return _context.Room.Where(e => (e.HotelId == hotelId) &&
            (e.RoomTypeId == roomTypeId) && (e.IsFree == true)).ToList();
        }

        public async Task EditFreeByIdAsync(int roomId, int hotelId, DateTime? CheckOutDate)
        {
            if (CheckOutDate == null)
            {
                var room = await _context.Room.FindAsync(roomId, hotelId);
                if (room != null)
                {
                    room.IsFree = !room.IsFree;
                    _context.Room.Update(room);
                    await _context.SaveChangesAsync();
                }
            }
        }                

        public async Task<IActionResult> MultiSearch(string hotelState, string roomTypeName, int price) {           
            var rooms = _context.Room.Include(r => r.Hotel).Include(r => r.RoomType).
                Where(r => r.Hotel.State.Equals(hotelState) &&
                r.RoomType.Name.Equals(roomTypeName) &&
                r.RoomType.PriceForNight <= price).
                OrderBy(r => r.HotelId).ThenBy(r => r.Id);

            return View("Index", await rooms.ToListAsync());
        }
    }
}
