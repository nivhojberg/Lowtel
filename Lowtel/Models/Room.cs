using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Lowtel.Models
{
    public class Room
    {
        [Display(Name = "Room Number")]
        public int Id { get; set; }
        [Display(Name = "Hotel Id")]
        public int HotelId { get; set; }
        [Display(Name = "Room Type Id")]
        public int RoomTypeId { get; set; }
        [Display(Name = "Is Free")]
        public bool IsFree{ get; set; }

        public Hotel Hotel { get; set; }
        [Display(Name = "Room Type")]
        public RoomType RoomType { get; set; }
    }
}
