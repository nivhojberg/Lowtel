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
    public class ReservationsController : Controller
    {
        private readonly LotelContext _context;

        public ReservationsController(LotelContext context)
        {
            _context = context;
        }

        // GET: Reservations
        public async Task<IActionResult> Index()
        {
            var lotelContext = _context.Reservation.Include(r => r.Client).Include(r => r.Hotel).Include(r => r.Room);
            return View(await lotelContext.ToListAsync());
        }

        // GET: Reservations/Details/5
        public IActionResult Details(string ClientId, int HotelId, int RoomId, DateTime CheckInDate)
        {
           
            CheckInDate = DateTime.Parse(ModelState["CheckInDate"].AttemptedValue);
            

            var reservation = _context.Reservation.Where(e => (e.CheckInDate == CheckInDate) && (e.ClientId == ClientId) && (e.HotelId == HotelId) && (e.RoomId == RoomId)).ToList();

            if (reservation.Count == 0)
            {
                return NotFound();
            }

            var hotel = _context.Hotel.Where(e => (e.Id == HotelId)).ToList() ;
            ViewData["CheckInDate"] = CheckInDate;

            return View(reservation[0]);
        }

        // GET: Reservations/Create
        public IActionResult Create()
        {
            TempData["ErrMessageReservation"] = "";
            ViewData["HotelId"] = new SelectList(_context.Hotel, "Id", "Name");
            ViewData["RoomTypeId"] = new SelectList(_context.RoomType, "Id", "Name");
            ViewData["ClientId"] = new SelectList(_context.Client, "Id", "Id");
            ViewData["RoomId"] = new SelectList(_context.Room, "Id", "Id");
            return View();
        }

        // POST: Reservations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ClientId,HotelId,RoomId,CheckInDate,CheckOutDate")] Reservation reservation)
        {
            bool IsReservationExist = IsClientHasOpenReservation(reservation);

            if(reservation.CheckInDate < DateTime.Today)
            {
                TempData["ErrMessageReservation"] = "Invalid Date!";
            }
            else if ((ModelState.IsValid) && (!IsReservationExist))
            {
                RoomsController room = new RoomsController(_context);
                _context.Add(reservation);
                await room.EditFreeByIdAsync(reservation.RoomId, reservation.HotelId, reservation.CheckOutDate);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["ErrMessageReservation"] = "There is an open reservation for this client on this date!";
            }

            ViewData["HotelId"] = new SelectList(_context.Hotel, "Id", "Name");
            ViewData["RoomTypeId"] = new SelectList(_context.RoomType, "Id", "Name");
            ViewData["ClientId"] = new SelectList(_context.Client, "Id", "Id");
            ViewData["RoomId"] = new SelectList(_context.Room, "Id", "Id");
            ViewData["CheckInDate"] = reservation.CheckInDate;

            return View(reservation);
        }

        // GET: Reservations/Edit/5
        public IActionResult Edit(string ClientId, int HotelId, int RoomId, DateTime CheckInDate)
        {
            
            CheckInDate = DateTime.Parse(ModelState["CheckInDate"].AttemptedValue);
            
            var reservation = _context.Reservation.Where(e => (e.CheckInDate == CheckInDate) && (e.ClientId == ClientId) && (e.HotelId == HotelId) && (e.RoomId == RoomId)).ToList();

            if (reservation.Count == 0)
            {
                return NotFound();
            }
            else
            {
                PriceCompute(reservation[0]);
            }
            ViewData["CheckInDate"] = CheckInDate;

            return View(reservation[0]);
        }

        // POST: Reservations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("ClientId,HotelId,RoomId,CheckInDate,CheckOutDate")] Reservation reservation)
        {
            reservation.CheckOutDate = DateTime.Today;
            RoomsController room = new RoomsController(_context);

            if (reservation.CheckInDate > reservation.CheckOutDate)
            {
                TempData["ErrMessageReservation"] = "Invalid dates range!";
            }

            else if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reservation);
                    await room.EditFreeByIdAsync(reservation.RoomId, reservation.HotelId, null);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReservationExists(reservation.ClientId))
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
            else
            {
                TempData["ErrMessageReservation"] = "There is an open reservation for this client on this date!";
            }

            ViewData["ClientId"] = new SelectList(_context.Client, "Id", "Id", reservation.ClientId);
            ViewData["HotelId"] = new SelectList(_context.Hotel, "Id", "Name", reservation.HotelId);
            ViewData["RoomId"] = new SelectList(_context.Room, "Id", "Id", reservation.RoomId);
            ViewData["CheckInDate"] = reservation.CheckInDate;
            return View(reservation);
        }

        // GET: Reservations/Delete/5
        public IActionResult Delete(string ClientId, int HotelId, int RoomId, DateTime CheckInDate)
        {
            CheckInDate = DateTime.Parse(ModelState["CheckInDate"].AttemptedValue);

            var reservation = _context.Reservation.Where(e => (e.CheckInDate == CheckInDate) && (e.ClientId == ClientId) && (e.HotelId == HotelId) && (e.RoomId == RoomId)).ToList();

            if (reservation.Count == 0)
            {
                return NotFound();
            }

            var hotel = _context.Hotel.Where(e => (e.Id == HotelId)).ToList();
            ViewData["CheckInDate"] = CheckInDate;
            return View(reservation[0]);
        }

        // POST: Reservations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed([Bind("ClientId,HotelId,RoomId,CheckInDate,CheckOutDate")] Reservation reservation)
        {
            _context.Reservation.Remove(reservation);
            RoomsController room = new RoomsController(_context);
            await room.EditFreeByIdAsync(reservation.RoomId, reservation.HotelId, reservation.CheckOutDate);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReservationExists(string id)
        {
            return _context.Reservation.Any(e => e.ClientId == id);
        }

        public void PriceCompute([Bind("ClientId,HotelId,RoomId,CheckInDate,CheckOutDate")] Reservation reservation)
        {
            reservation.CheckOutDate = DateTime.Today;
            int price = 0;
            int nights = 0;
            int pricePerNight = 0;
            if (((!reservation.CheckOutDate.ToString().Equals("01/01/0001 00:00:00")) &&
                (!reservation.CheckInDate.ToString().Equals("01/01/0001 00:00:00"))) &&
                reservation.CheckInDate < reservation.CheckOutDate)
            {
                if (reservation.CheckOutDate != null)
                {
                    TimeSpan totalDays = ((DateTime)(reservation.CheckOutDate)).Subtract(reservation.CheckInDate);
                    nights = totalDays.Days + 1;
                    var room = _context.Room.Where(e => (e.Id == reservation.RoomId) && (e.HotelId == reservation.HotelId)).ToList();
                    int roomTypeId = room[0].RoomTypeId;
                    var roomType = _context.RoomType.Where(e => (e.Id == roomTypeId)).ToList();
                    pricePerNight = roomType[0].PriceForNight;
                    price = pricePerNight * nights;
                }
            }
            
            ViewBag.price = price;
            ViewBag.nights = nights;
            ViewBag.pricePerNight = pricePerNight;
        }

        public bool IsClientHasOpenReservation(Reservation reservation)
        {
            var rsvList = _context.Reservation.Where(e => (e.CheckInDate == reservation.CheckInDate) && (e.ClientId == reservation.ClientId) && (e.CheckOutDate == null)).ToList();
            if (rsvList.Count > 0)
            {
                return true;
            }
            return false;

        }

    }
}
