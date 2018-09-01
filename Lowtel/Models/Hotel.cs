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
        public int Id { get; set; }
        [Display(Name = "Hotel Name")]
        public string Name { get; set; }
        [Display(Name = "Hotel State")]
        public string State { get; set; }
        [Display(Name = "Hotel Address")]
        public string Address { get; set; }        
        [Display(Name = "Hotel Stars Rate")]
        public int StarsRate { get; set; }
        [Display(Name = "Hotel Description")]
        public string Description { get; set; }
    }
}
