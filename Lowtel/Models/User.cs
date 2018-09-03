using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Lowtel.Models
{
    public class User
    {
        [Required, MaxLength(20)]
        public string FirstName { get; set; }

        [Required, MaxLength(20)]
        public string LastName { get; set; }

        [Required, MaxLength(20)]
        public string Email { get; set; }

        [Required, DataType(DataType.Password),MinLength(6),MaxLength(10)]
        public string Password { get; set; }
    }
}
