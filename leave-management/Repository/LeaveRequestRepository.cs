using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using leave_management.Contracts;
using leave_management.Data;
using Microsoft.EntityFrameworkCore;

namespace leave_management.Repository
{
    public class LeaveRequestRepository : ILeaveRequestRepository
    {
        private readonly ApplicationDbContext _db;
        public LeaveRequestRepository(ApplicationDbContext db)
        {
            _db = db;
        }
        public bool Create(LeaveRequest entity)
        {
            _db.LeaveRequests.Add(entity);
            return Save();
        }

        public bool Delete(LeaveRequest entity)
        {
            _db.LeaveRequests.Remove(entity);
            return Save();
        }

        public ICollection<LeaveRequest> FindAll()
        {
            return _db.LeaveRequests.Include(q=>q.RequestingEmployee).Include(q=>q.LeaveType).Include(q=>q.ApprovedBy).ToList();
        }

        public LeaveRequest FindById(int id)
        {
            return _db.LeaveRequests.Include(q => q.RequestingEmployee).Include(q => q.LeaveType).Include(q => q.ApprovedBy).FirstOrDefault(q=>q.Id==id);
        }

        public ICollection<LeaveRequest> GetLeaveRequestsByEmploee(string employeeId)
        {
            var leaveRequest = FindAll().Where(q => q.RequestingEmployeeId == employeeId).ToList();
            return leaveRequest;
        }

        public bool isExists(int id)
        {
            var exists = _db.LeaveRequests.Any(q => q.Id == id);
            return exists;
        }

        public bool Save()
        {
            var change=_db.SaveChanges();
            return change > 0;
        }

        public bool Update(LeaveRequest entity)
        {
            _db.LeaveRequests.Update(entity);
            return Save();
        }
    }
}
