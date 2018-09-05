﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Lowtel.Models
{
    public class User
    {
        [Required, MaxLength(20)]
        public string UserName { get; set; }

        [Required, DataType(DataType.Password),MinLength(6),MaxLength(10)]
        public string Password { get; set; }
    }
}
