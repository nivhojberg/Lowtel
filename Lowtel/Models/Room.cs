using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lowtel.Models
{
    public struct RoomExtendData
    {
        public int roomId;
        public int hotelId;
        public int roomTypeId;
        public string hotelName;
        public string roomTypeName;
        public int PriceForNight;
        public bool isFree;
    }

    public class Room
    {
        public int Id { get; set; }
        public int HotelId { get; set; }
        public int RoomTypeId { get; set; }
        public bool isFree{ get; set; }
    }
}
