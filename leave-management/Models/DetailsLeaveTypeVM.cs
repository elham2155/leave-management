﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace leave_management.Models
{
    public class DetailsLeaveTypeVM
    {
        
        public int Id { get; set; }    

        [Required]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Default Number of Days")]
        [Range(1,25,ErrorMessage ="Out of Range 1-25")]
        public int DefaultDays { get; set; }

        [Display(Name ="Date Created")]
        public DateTime DateCreated { get; set; }
    }
    public class CreateLeaveTypeVM
    {  
        [Required]
        public string Name { get; set; }        
    }
}
