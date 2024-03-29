﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using leave_management.Contracts;
using leave_management.Data;
using Microsoft.EntityFrameworkCore;

namespace leave_management.Repository
{
    public class LeaveAllocationRepository : ILeaveAllocationRepository
    {
        private readonly ApplicationDbContext _db;
        public LeaveAllocationRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<bool> CheckAllocation(int leaveTypeId, string employeeId)
        {
            var period=DateTime.Now.Year;
            var findAllocation = await FindAll();
            return findAllocation.Where(q => q.LeaveTypeId == leaveTypeId && q.EmployeeId == employeeId && q.Period == period).Any();
        }

        public async Task<bool> Create(LeaveAllocation entity)
        {
            await _db.LeaveAllocations.AddAsync(entity);
            return await Save();
        }

        public async Task<bool> Delete(LeaveAllocation entity)
        {
            _db.LeaveAllocations.Remove(entity);
            return await Save();
        }

        public async Task<ICollection<LeaveAllocation>> FindAll()
        {
            return await _db.LeaveAllocations.Include(q=>q.LeaveType).Include(q=>q.Employee).ToListAsync();
        }

        public async Task<LeaveAllocation> FindById(int id)
        {
            return await _db.LeaveAllocations.Include(q => q.LeaveType).Include(q => q.Employee).FirstOrDefaultAsync(q=>q.Id==id);
        }

        public async Task<ICollection<LeaveAllocation>> GetLeaveAllocationsByEmploee(string employeeId)
        {
            var period = DateTime.Now.Year;
            var leaveAllocations = await FindAll();
            return leaveAllocations.Where(q => q.EmployeeId == employeeId && q.Period == period).ToList();
        }

        public async Task<LeaveAllocation> GetLeaveAllocationsByEmploeeAndType(string employeeId, int leaveTypeid)
        {
            var period = DateTime.Now.Year;
            var leaveAllocations = await FindAll();
            return leaveAllocations.FirstOrDefault(q => q.EmployeeId == employeeId && q.LeaveTypeId == leaveTypeid && q.Period == period);
        }

        public async Task<bool> isExists(int id)
        {
            var exists = await _db.LeaveAllocations.AnyAsync(q=>q.Id==id);
            return exists;
        }

        public async Task<bool> Save()
        {
            var change = await _db.SaveChangesAsync();
            return change > 0;
        }

        public async Task<bool> Update(LeaveAllocation entity)
        {
            _db.LeaveAllocations.Update(entity);
            return await Save();
        }
    }
}
