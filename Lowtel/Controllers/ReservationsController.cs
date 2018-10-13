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
using Microsoft.ML.Legacy;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Runtime.Api;
using Microsoft.ML.Trainers;
using Microsoft.ML.Transforms;
using Microsoft.ML.Legacy.Data;
using Microsoft.ML.Legacy.Transforms;
using Microsoft.ML.Legacy.Trainers;
using System.IO;

namespace Lowtel.Controllers
{    
    public class ReservationsController : Controller
    {        
        private readonly LotelContext _context;
        public string dataPath = "ML/train.txt";

        public ReservationsController(LotelContext context)
        {
            _context = context;
        }

        // GET: Reservations
        public async Task<IActionResult> Index(string searchString)
        {
            if (HttpContext.Session.GetString(UsersController.SessionName) != null)
            {
                if (CountOfRoomTypeOnReservations() > 1)
                {
                    TrainReservationsData();
                }

                IQueryable<Reservation> reservations = _context.Set<Reservation>();

                reservations = reservations.Include(r => r.Client).
                    Include(r => r.Hotel).
                    Include(r => r.Room).
                    Include(r => r.Room.RoomType);

                if (!String.IsNullOrEmpty(searchString))
                {
                    int numberSearch;

                    reservations = reservations.Where(r =>
                    r.Hotel.Name.Contains(searchString) ||
                    r.Hotel.State.Contains(searchString) ||
                    r.Client.Id.Contains(searchString) ||
                    (Int32.TryParse(searchString, out numberSearch) && r.RoomId == numberSearch));
                }

                reservations = reservations.OrderByDescending(r => r.CheckInDate);                

                return View(await reservations.ToListAsync());
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Reservations/Details/5
        public IActionResult Details(string ClientId, int HotelId, int RoomId, DateTime CheckInDate)
        {

            CheckInDate = DateTime.Parse(ModelState["CheckInDate"].AttemptedValue);


            Reservation reservation = _context.Reservation.Include(r => r.Client).Include(r => r.Hotel).Include(r => r.Room).Where(e => (e.CheckInDate.Equals(CheckInDate)) && (e.ClientId == ClientId) && (e.HotelId == HotelId) && (e.RoomId == RoomId)).FirstOrDefault();

            if (reservation == null)
            {
                return NotFound("Reservation was not found");
            }
            else
            {
                reservation.Room.RoomType = _context.RoomType.Where(r => r.Id == reservation.Room.RoomTypeId).FirstOrDefault();
            }


            ViewData["CheckInDate"] = CheckInDate;

            return View(reservation);
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
            if (HttpContext.Session.GetString(UsersController.SessionName) != null)
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
            else
            {
                return RedirectToAction("Index", "Home");
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
            if (HttpContext.Session.GetString(UsersController.SessionName) != null)
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
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Reservations/Delete/5
        public IActionResult Delete(string ClientId, int HotelId, int RoomId, DateTime CheckInDate)
        {
            CheckInDate = DateTime.Parse(ModelState["CheckInDate"].AttemptedValue);

            var reservation = _context.Reservation.Where(e => (e.CheckInDate.Equals(CheckInDate)) && (e.ClientId == ClientId) && (e.HotelId == HotelId) && (e.RoomId == RoomId)).ToList();

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

            if (HttpContext.Session.GetString(UsersController.SessionName) != null)
            {
                _context.Reservation.Remove(reservation);
                RoomsController room = new RoomsController(_context);
                await room.EditFreeByIdAsync(reservation.RoomId, reservation.HotelId, reservation.CheckOutDate);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
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

                // In case the room has not found
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

        public int CountOfRoomTypeOnReservations()
        {
            return _context.Reservation.Include(r => r.Room).GroupBy(r => r.Room.RoomTypeId).Count();
        }

        public List<IGrouping<int, Reservation>> GetRoomTypeOnReservations()
        {
            return _context.Reservation.Include(r => r.Room).GroupBy(r => r.Room.RoomTypeId).ToList();            
        }

        // Getting hotel id and calculate by ML recommendation for favorite room type.
        public IActionResult GetRecommendedRoomTypeByHotelId(int id)
        {           
            string state = _context.Hotel.Select(h => h.State).FirstOrDefault();

            if (state == null)
            {
                return NotFound("Hotel state not found for hotel with id: " + id);
            }
            else
            {
                int roomTypeId;
                var RoomTypeList = GetRoomTypeOnReservations();

                if (RoomTypeList.Count == 1)
                {
                    roomTypeId = RoomTypeList[0].Key;
                }
                else if (RoomTypeList.Count > 1)
                {
                    roomTypeId = PredictRoomByReservation(id, state.GetHashCode());
                }
                else
                {
                    return BadRequest();
                }
                    
                string predictedRoomType = _context.RoomType.Where(r => r.Id == roomTypeId).Select(r => r.Name).FirstOrDefault();

                if (predictedRoomType == null)
                {
                    return NotFound("There is no room type id " + roomTypeId);
                }

                return Ok(predictedRoomType);
            }
        }

        // Getting two features and predict favorite room type id (label).
        public int PredictRoomByReservation(float hotelId, float hotelStateId)
        {
            // Create a pipeline and load your data.
            var pipeline = new LearningPipeline();
            
            // Load the data from the training file by path.
            pipeline.Add(new TextLoader(this.dataPath).CreateFrom<TrainData>(separator: ','));

            // Assign numeric values to text in the "Label" column, because only
            // numbers can be processed during model training
            pipeline.Add(new Dictionarizer("Label"));

            // Puts all features into a vector
            pipeline.Add(new ColumnConcatenator("Features", "HotelId", "HotelStateId"));

            // Add learner
            // Add a learning algorithm to the pipeline. 
            // This is a classification scenario.
            pipeline.Add(new StochasticDualCoordinateAscentClassifier());

            // Convert the Label back into original text (after converting to number).
            pipeline.Add(new PredictedLabelColumnOriginalValueConverter() { PredictedLabelColumn = "PredictedLabel" });

            // Train our model based on the data set.
            var model = pipeline.Train<TrainData, Prediction>();

            // Use our model to make a prediction.
            return model.Predict(new TrainData()
            {
                HotelId = hotelId,
                HotelStateId = hotelStateId
            }).RoomTypeId;

        }

        // This function prepare file with training data.
        public void TrainReservationsData()
        {
            var reservations = _context.Reservation.Include(r => r.Hotel).Include(r => r.Room).
                Select(r => new { r.HotelId, r.Hotel.State, r.Room.RoomTypeId }).ToList();
            using (StreamWriter outputFile = new StreamWriter(this.dataPath))
            {
                foreach (var reservation in reservations)
                {
                    int hotelId = reservation.HotelId;
                    int stateId = reservation.State.GetHashCode();
                    outputFile.WriteLine(hotelId + "," + stateId + "," + reservation.RoomTypeId);
                }

                outputFile.Close();
            } 
        }

    }

    // This class is the train data vector structure.
    public class TrainData
    {
        [Column("0")]
        [ColumnName("HotelId")]
        public float HotelId;

        [Column("1")]
        [ColumnName("HotelStateId")]
        public float HotelStateId;

        [Column("2")]
        [ColumnName("Label")]
        public int RoomTypeId;
    }

    // This clss is the prediction object.
    public class Prediction
    {
        [ColumnName("PredictedLabel")]
        public int RoomTypeId; 
    }

}
