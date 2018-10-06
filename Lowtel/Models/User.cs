using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Lowtel.Models
{
    public class User
    {
        [Required, MaxLength(20), Display(Name = "User Name")]
        public string UserName { get; set; }

        [Required, DataType(DataType.Password), MinLength(3), MaxLength(10)]
        public string Password { get; set; }
    }
}
