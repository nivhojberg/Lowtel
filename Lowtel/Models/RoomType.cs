using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Lowtel.Models
{
    public class RoomType
    {
        [Display(Name = "Room Type Id")]
        public int Id { get; set; }
        [Display(Name = "Room Type Name")]
        public string Name { get; set; }
        [Display(Name = "Room Type Description")]
        public string Description { get; set; }
        [Display(Name = "Price For Night ($)")]
        public int PriceForNight { get; set; }        
    }
}
