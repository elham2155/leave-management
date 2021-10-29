using System;
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

        public bool CheckAllocation(int leaveTypeId, string employeeId)
        {
            var period=DateTime.Now.Year;
            return FindAll().Where(q => q.LeaveTypeId == leaveTypeId && q.EmployeeId == employeeId && q.Period == period).Any();
        }

        public bool Create(LeaveAllocation entity)
        {
            _db.LeaveAllocations.Add(entity);
            return Save();
        }

        public bool Delete(LeaveAllocation entity)
        {
            _db.LeaveAllocations.Remove(entity);
            return Save();
        }

        public ICollection<LeaveAllocation> FindAll()
        {
            return _db.LeaveAllocations.Include(q=>q.LeaveType).Include(q=>q.Employee).ToList();
        }

        public LeaveAllocation FindById(int id)
        {
            return _db.LeaveAllocations.Include(q => q.LeaveType).Include(q => q.Employee).FirstOrDefault(q=>q.Id==id);
        }

        public ICollection<LeaveAllocation> GetLeaveAllocationsByEmploee(string employeeId)
        {
            var period = DateTime.Now.Year;
            return FindAll().Where(q=>q.EmployeeId== employeeId && q.Period == period).ToList();
        }

        public LeaveAllocation GetLeaveAllocationsByEmploeeAndType(string employeeId, int leaveTypeid)
        {
            var period = DateTime.Now.Year;
            return FindAll().FirstOrDefault(q => q.EmployeeId == employeeId && q.LeaveTypeId==leaveTypeid && q.Period == period);
        }

        public bool isExists(int id)
        {
            var exists = _db.LeaveAllocations.Any(q=>q.Id==id);
            return exists;
        }

        public bool Save()
        {
            var change = _db.SaveChanges();
            return change > 0;
        }

        public bool Update(LeaveAllocation entity)
        {
            _db.LeaveAllocations.Update(entity);
            return Save();
        }
    }
}
