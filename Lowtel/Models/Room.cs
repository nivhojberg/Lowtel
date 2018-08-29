using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lowtel.Models
{
    public class Room
    {
        public int Id { get; set; }
        public int HotelId { get; set; }
        public int RoomTypeId { get; set; }
        public bool isFree{ get; set; }
    }
}
