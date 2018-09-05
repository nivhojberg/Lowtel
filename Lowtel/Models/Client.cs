using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Lowtel.Models
{
    public class Client
    {
        [Display(Name = "Client Id")]
        [StringLength(10, MinimumLength = 9)]
        [Required]        
        public string Id { get; set; }


        [Display(Name = "First Name")]
        [StringLength(10, MinimumLength = 2)]
        [Required]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]        
        [StringLength(10, MinimumLength = 2)]
        [Required]
        public string LastName { get; set; }


        [Display(Name = "Phone Number")]        
        [StringLength(20)]
        [Required]
        public string PhoneNumber { get; set; }


        [Display(Name = "Credit Card")]        
        [StringLength(20)]
        [Required]
        public string CreditCard { get; set; }        
    }
}
