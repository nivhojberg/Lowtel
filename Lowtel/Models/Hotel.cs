using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Lowtel.Models
{
    public class Hotel
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }
        public string Address { get; set; }        
        public int StarsRate { get; set; }
        public string Description { get; set; }
    }
}
