using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Lowtel.Models
{
    public class Reservation
    {
        [Display(Name = "Client Id")]
        public string ClientId { get; set; }
        [Display(Name = "Hotel Id")]
        public int HotelId { get; set; }
        [Display(Name = "Room Id")]
        public int RoomId { get; set; }
        [Display(Name = "Check In Date")]
        public DateTime CheckInDate { get; set; }
        [Display(Name = "Check Out Date")]
        public DateTime CheckOutDate { get; set; }

        public Hotel Hotel { get; set; }
        public Room Room { get; set; }
        public Client Client { get; set; }
    }
}
