using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using leave_management.Contracts;
using leave_management.Data;
using leave_management.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace leave_management.Controllers
{
    [Authorize]
    public class LeaveRequestController : Controller
    {
        private readonly ILeaveRequestRepository _leaveRequestRepo;
        private readonly IMapper _mapper;
        private readonly UserManager<Employee> _userManager;
        private readonly ILeaveTypeRepository _leaveTypeRepo;
        private readonly ILeaveAllocationRepository _leaveAllocationRepo;

        public LeaveRequestController(ILeaveRequestRepository leaveRequestRepo, IMapper mapper, 
                                 UserManager<Employee> userManager, ILeaveTypeRepository leaveTypeRepo,
                                 ILeaveAllocationRepository leaveAllocationRepo)
        {
            _leaveRequestRepo = leaveRequestRepo;
            _mapper = mapper;
            _userManager = userManager;
            _leaveTypeRepo = leaveTypeRepo;
            _leaveAllocationRepo = leaveAllocationRepo;
        }
        // GET: LeaveRequestController
        [Authorize(Roles = "Administrator")]
        public ActionResult Index()
        {
            var leaveRequests = _leaveRequestRepo.FindAll();
            var leaveRequestsModel = _mapper.Map<List<LeaveRequestVM>>(leaveRequests);
            var model = new AdminLeaveRequestViewVM 
            { 
                TotalRequests=leaveRequestsModel.Count,
                ApprovedRequests= leaveRequestsModel.Count(q => q.Approved == true), //leaveRequestsModel.Where(q=>q.Approved==true).Count(),
                PendingRequests = leaveRequestsModel.Count(q => q.Approved == null),
                RejectedRequests = leaveRequestsModel.Count(q => q.Approved == false),
                LeaveRequests=leaveRequestsModel
            };
            return View(model);
        }

        // GET: LeaveRequestController/Details/5
        public ActionResult Details(int id)
        {
            var leaveRequest = _leaveRequestRepo.FindById(id);
            var model = _mapper.Map<LeaveRequestVM>(leaveRequest);
            return View(model);
        }

        // GET: LeaveRequestController/Create
        public ActionResult Create()
        {
            var leaveTypes = _leaveTypeRepo.FindAll();
            var leaveTypeItems = leaveTypes.Select(q => new SelectListItem 
            { 
                Text=q.Name, 
                Value=q.Id.ToString(),
            });
            var model = new CreateLeaveRequestVM
            {              
                LeaveTypes=leaveTypeItems
            };
            return View(model);
        }

        // POST: LeaveRequestController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateLeaveRequestVM model)
        {
            try
            {
                var StartDate = Convert.ToDateTime(model.StartDate);
                var EndDate = Convert.ToDateTime(model.EndDate);

                var leaveTypes = _leaveTypeRepo.FindAll();
                var leaveTypeItems = leaveTypes.Select(q => new SelectListItem
                {
                    Text = q.Name,
                    Value = q.Id.ToString(),
                });
                model.LeaveTypes = leaveTypeItems;
                if (!ModelState.IsValid)
                {
                    return View(model);
                }
                //arrange time validation
                if (DateTime.Compare(StartDate,EndDate) > 1)
                {
                    ModelState.AddModelError("", "The start date can not be further in the future than end date!");
                    return View(model);
                }

                //loged in user
                var employee = _userManager.GetUserAsync(User).Result;

                var allocation = _leaveAllocationRepo.GetLeaveAllocationsByEmploeeAndType(employee.Id, model.LeaveTypeId);
                int dayesRequested = (int)(EndDate-StartDate).TotalDays;
                if (dayesRequested > allocation.NumberofDays)
                {
                    ModelState.AddModelError("", "You don't have sufficient days for this request!");
                    return View(model);
                }

                var leaveRequestModel = new LeaveRequestVM 
                { 
                    RequestingEmployeeId=employee.Id,
                    StartDate=StartDate,
                    EndDate=EndDate,
                    Approved=null,
                    DateRequested=DateTime.Now,
                    DateActioned = DateTime.Now,
                    LeaveTypeId=model.LeaveTypeId,
                    RequestComments=model.RequestComments
                };
                var leaveRequest = _mapper.Map<LeaveRequest>(leaveRequestModel);
                var isSuccess = _leaveRequestRepo.Create(leaveRequest);
                if (!isSuccess)
                {
                    ModelState.AddModelError("", "Sth. went wrong to submit!");
                    return View(model);
                }
                return RedirectToAction(nameof(MyLeave));
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", "Sth. went wrong with exeption!");
                return View(model);
            }
        }

        public ActionResult ApproveRequest(int id)
        {
            try
            {
                //loged in user
                var user = _userManager.GetUserAsync(User).Result;

                var leaveRequest = _leaveRequestRepo.FindById(id);

                var employeeId = leaveRequest.RequestingEmployeeId;

                var leaveTypeId = leaveRequest.LeaveTypeId;

                var allocation = _leaveAllocationRepo.GetLeaveAllocationsByEmploeeAndType(employeeId,leaveTypeId);
                int dayesRequested = (int)(leaveRequest.EndDate - leaveRequest.StartDate).TotalDays;
                allocation.NumberofDays = allocation.NumberofDays - dayesRequested;

                leaveRequest.Approved = true;
                leaveRequest.ApprovedById = user.Id;
                leaveRequest.DateActioned = DateTime.Now;

                _leaveRequestRepo.Update(leaveRequest);
                _leaveAllocationRepo.Update(allocation);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return RedirectToAction(nameof(Index));
            }         
            
        }

        public ActionResult RejectRequest(int id)
        {
            try
            {
                //loged in user
                var user = _userManager.GetUserAsync(User).Result;

                var leaveRequest = _leaveRequestRepo.FindById(id);

                leaveRequest.Approved = false;
                leaveRequest.ApprovedById = user.Id;
                leaveRequest.DateActioned = DateTime.Now;

                _leaveRequestRepo.Update(leaveRequest);
                
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {

                return RedirectToAction(nameof(Index));
            }           
        }

        public ActionResult MyLeave()
        {
            var employee = _userManager.GetUserAsync(User).Result;
            var employeeId = employee.Id;
            var employeeAllocations = _leaveAllocationRepo.GetLeaveAllocationsByEmploee(employeeId);
            var employeeRequests = _leaveRequestRepo.GetLeaveRequestsByEmploee(employeeId);

            var employeeAllocationsModel = _mapper.Map<List<LeaveAllocationVM>>(employeeAllocations);
            var employeeRequestsModel = _mapper.Map<List<LeaveRequestVM>>(employeeRequests);

            var model = new EmployeeLeaveRequestViewVM
            { 
                LeaveAllocations=employeeAllocationsModel,
                LeaveRequests=employeeRequestsModel
            };

            return View(model);

        }

        public ActionResult CancelRequest(int id)
        {
            var leaveRequest = _leaveRequestRepo.FindById(id);
            leaveRequest.Cancelled = true;
            _leaveRequestRepo.Update(leaveRequest);
            return RedirectToAction("MyLeave");
        }
        // GET: LeaveRequestController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: LeaveRequestController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: LeaveRequestController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: LeaveRequestController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
