using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Lowtel.Models
{
    public class Hotel
    {
        [Display(Name = "Hotel Id")]
        [Required]
        public int Id { get; set; }

        [Display(Name = "Hotel Name")]
        [StringLength(50)]
        [Required]
        public string Name { get; set; }

        [Display(Name = "Hotel State")]
        [StringLength(15)]
        [Required]
        public string State { get; set; }

        [Display(Name = "Hotel Address")]
        [StringLength(100)]
        [Required]
        public string Address { get; set; }  
        
        [Display(Name = "Hotel Stars Rate")]
        [Range(3, 5)]
        [Required]
        public int StarsRate { get; set; }

        [Display(Name = "Hotel Description")]
        [StringLength(500)]        
        public string Description { get; set; }

        [Display(Name = "x-coordinate")]        
        [Required]
        public double CordX { get; set; }

        [Display(Name = "y-coordinate")]
        [Required]
        public double CordY { get; set; }
    }
}
