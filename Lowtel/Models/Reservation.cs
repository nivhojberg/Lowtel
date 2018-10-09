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
        [Required]
        public string ClientId { get; set; }

        [Display(Name = "Hotel Id")]
        [Required]
        public int HotelId { get; set; }

        [Display(Name = "Room Id")]
        [Range(1, 999)]
        [Required]
        public int RoomId { get; set; }

        [Display(Name = "Check-In Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy - HH:mm:ss}")]
        [Required]
        public DateTime CheckInDate { get; set; }

        [Display(Name = "Check-Out Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy - HH:mm:ss}")]
        public DateTime? CheckOutDate { get; set; }

        public Hotel Hotel { get; set; }
        public Room Room { get; set; }
        public Client Client { get; set; }
    }
}
