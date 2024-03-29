﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using leave_management.Data;
using leave_management.Models;

namespace leave_management.Mapping
{
    public class Maps : Profile
    {
        public Maps()
        {
            CreateMap<LeaveType, DetailsLeaveTypeVM>().ReverseMap();
            CreateMap<LeaveType, CreateLeaveTypeVM>().ReverseMap();
            //----------------------------------------------------------------------
            CreateMap<LeaveRequest, LeaveRequestVM>().ReverseMap();
            //CreateMap<LeaveRequest, AdminLeaveRequestViewVM>().ReverseMap();
            //----------------------------------------------------------------------
            CreateMap<LeaveAllocation, LeaveAllocationVM>().ReverseMap();
            CreateMap<LeaveAllocation, EditLeaveAllocationVM>().ReverseMap();
            //----------------------------------------------------------------------
            CreateMap<Employee, EmployeeVM>().ReverseMap();
            
        }
    }
}
