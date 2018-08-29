using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Lowtel.Models
{
    public class Reservation
    {                
        public string ClientId { get; set; }        
        public int HotelId { get; set; }        
        public int RoomId { get; set; }

        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
    }
}
