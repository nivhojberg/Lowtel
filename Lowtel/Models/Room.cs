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
        [Range(1, 999)]
        [Required]
        public int Id { get; set; }

        [Display(Name = "Hotel Id")]
        [Required]
        public int HotelId { get; set; }

        [Display(Name = "Room-Type Id")]
        [Required]
        public int RoomTypeId { get; set; }

        [Display(Name = "Is Free")]
        [Required]        
        public bool IsFree{ get; set; }

        public Hotel Hotel { get; set; }

        [Display(Name = "Room-Type")]
        public RoomType RoomType { get; set; }

        public Room()
        {
            this.Id = 1;
            this.IsFree = true;
        }
    }
}
