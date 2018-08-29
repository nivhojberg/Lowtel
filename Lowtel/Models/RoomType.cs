using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Lowtel.Models
{
    public class RoomType
    {        
        public int Id { get; set; }    
        public string Name { get; set; }
        public string Description { get; set; }
        public int PriceForNight { get; set; }
    }
}
