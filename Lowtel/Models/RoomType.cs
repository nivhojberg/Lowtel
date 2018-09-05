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
        [Required]
        public int Id { get; set; }

        [Display(Name = "Room Type Name")]
        [StringLength(50)]
        [Required]
        public string Name { get; set; }

        [Display(Name = "Room Type Description")]
        [StringLength(500)]
        public string Description { get; set; }

        [Display(Name = "Price For Night ($)")]
        [Range(1, 10000)]
        [Required]
        public int PriceForNight { get; set; }        
    }
}
