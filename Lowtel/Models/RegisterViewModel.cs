using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Lowtel.Models
{
    public class RegisterViewModel
    {
        [Required, MaxLength(20)]
        public string FirstName { get; set; }

        [Required, MaxLength(20)]
        public string LastName { get; set; }

        [Required, MaxLength(20)]
        public string Email { get; set; }

        [Required, DataType(DataType.Password),MinLength(6),MaxLength(10)]
        public string Password { get; set; }

        [DataType(DataType.Password), Compare(nameof(Password),ErrorMessage = "The password does not match the confirmation password")]
        public string ConfirmPassword { get; set; }
    }
}
