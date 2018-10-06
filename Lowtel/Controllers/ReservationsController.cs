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
            var lotelContext = _context.Reservation.Include(r => r.Client).Include(r => r.Hotel).Include(r => r.Room).OrderByDescending(r => r.CheckInDate);
            return View(await lotelContext.ToListAsync());
        }

        // GET: Reservations/Details/5
        public IActionResult Details(string ClientId, int HotelId, int RoomId, DateTime CheckInDate)
        {
           
            CheckInDate = DateTime.Parse(ModelState["CheckInDate"].AttemptedValue);            
            

            var reservation = _context.Reservation.Where(e => (e.CheckInDate.Equals(CheckInDate)) && (e.ClientId == ClientId) && (e.HotelId == HotelId) && (e.RoomId == RoomId)).ToList();

            if (reservation.Count == 0)
            {
                return NotFound("Reservation was not found");
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
        public async Task<IActionResult> Create([Bind("ClientId,HotelId,RoomId,CheckOutDate")] Reservation reservation)
        {
            reservation.CheckInDate = DateTime.Now;
            reservation.CheckInDate = reservation.CheckInDate.AddMilliseconds(-1 * reservation.CheckInDate.Millisecond);            

            if (!ModelState.IsValid)
            {
                 return BadRequest("Reservation parameters are not valid");
            }         
            else
            {
                RoomsController roomsController = new RoomsController(_context);                
                await roomsController.EditFreeByIdAsync(reservation.RoomId, reservation.HotelId, reservation.CheckOutDate);
                _context.Add(reservation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }            
        }

        // GET: Reservations/Edit/5
        public IActionResult Edit(string ClientId, int HotelId, int RoomId, DateTime CheckInDate)
        {
            
            CheckInDate = DateTime.Parse(ModelState["CheckInDate"].AttemptedValue);
            
            var reservation = _context.Reservation.Where(e => (e.CheckInDate.Equals(CheckInDate)) && (e.ClientId == ClientId) && (e.HotelId == HotelId) && (e.RoomId == RoomId)).FirstOrDefault();

            if (reservation == null)
            {
                return NotFound("Reservation was not found");
            }
            else if (!PriceCompute(reservation))
            {

                return NotFound("Error on price computation");
            }

            ViewData["CheckInDate"] = CheckInDate;

            return View(reservation);
        }

        // POST: Reservations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("ClientId,HotelId,RoomId,CheckInDate,CheckOutDate")] Reservation reservation)
        {
            reservation.CheckOutDate = DateTime.Now;
            RoomsController room = new RoomsController(_context);

            if (ModelState.IsValid)
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

        private bool PriceCompute(Reservation reservation)
        {
            reservation.CheckOutDate = DateTime.Now;
            
            // Data initialize
            int price = 0;
            int nights = 0;
            int pricePerNight = 0;

            // Check in and check out dates validations.
            if ((reservation.CheckInDate != null) && (reservation.CheckOutDate != null) &&
                (!reservation.CheckOutDate.ToString().Equals("01/01/0001 00:00:00")) &&
                (!reservation.CheckInDate.ToString().Equals("01/01/0001 00:00:00")) &&
                reservation.CheckInDate < reservation.CheckOutDate)
            {
                TimeSpan totalDays = ((DateTime)(reservation.CheckOutDate)).Subtract(reservation.CheckInDate);
                nights = totalDays.Days + 1;

                // Query the reservation room object from DV.
                Room room = _context.Room.Where(e => (e.Id == reservation.RoomId) && (e.HotelId == reservation.HotelId)).Include(r => r.RoomType).FirstOrDefault();   
                
                // In case the room has not found.
                if (room == null)
                {
                    return false;
                }

                pricePerNight = room.RoomType.PriceForNight;
                price = pricePerNight * nights;

                ViewBag.price = price;
                ViewBag.nights = nights;
                ViewBag.pricePerNight = pricePerNight;

                return true;
            }
            else
            {
                return false;
            }                       
        }

    }
}
