using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using leave_management.Data;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace leave_management.Models
{
    public class LeaveAllocationVM
    {
        public int Id { get; set; }
        
        public int NumberofDays { get; set; }
        public DateTime DateCreated { get; set; }
        
        public EmployeeVM Employee { get; set; }
        public string EmployeeId { get; set; }
        //public IEnumerable<SelectListItem> Employees { get; set; }

        public DetailsLeaveTypeVM LeaveType { get; set; }
        public int LeaveTypeId { get; set; }
        //public IEnumerable<SelectListItem> LeaveTypes { get; set; }

        public int Period { get; set; }
    }

    public class CreateLeaveAllocationVM
    {
        public int NumberUpdated { get; set; }
        public List<DetailsLeaveTypeVM> LeaveTypes { get; set; }
    }

    public class ViewAllocationVM
    {
        public EmployeeVM Employee { get; set; }
        public string EmployeeId { get; set; }
        public List<LeaveAllocationVM> LeaveAllocations { get; set; }

    }
    public class EditLeaveAllocationVM
    {
        public int Id { get; set; }
        public EmployeeVM Employee { get; set; }
        public string EmployeeId { get; set; }
        public int NumberofDays { get; set; }
        public DetailsLeaveTypeVM LeaveType { get; set; }
    }
}
