﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace leave_management.Data
{
    public class Employee: IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string TaxtId { get; set; }
        public DateTime DateofBirth { get; set; }
        public DateTime DateJoined { get; set; }

    }
}
