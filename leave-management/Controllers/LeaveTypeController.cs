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
using Microsoft.AspNetCore.Mvc;

namespace leave_management.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class LeaveTypeController : Controller
    {
        private readonly ILeaveTypeRepository _repo;
        private readonly IMapper _mapper;

        public LeaveTypeController(ILeaveTypeRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        // GET: LeaveTypeController
        public async Task<ActionResult> Index()
        {
            var leaveType =await _repo.FindAll();
            var model = _mapper.Map<List<LeaveType>,List<DetailsLeaveTypeVM>>(leaveType.ToList());
            return View(model);
        }

        // GET: LeaveTypeController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            var isExist = await _repo.isExists(id);
            if (!isExist)
            {
                return NotFound();
            }
            var leaveType =await _repo.FindById(id);
            var model = _mapper.Map<DetailsLeaveTypeVM>(leaveType);

            return View(model);
        }

        // GET: LeaveTypeController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: LeaveTypeController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(DetailsLeaveTypeVM model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var leaveType = _mapper.Map<LeaveType>(model);
                leaveType.DateCreated = DateTime.Now;

                var isSuccess =await _repo.Create(leaveType);
                if (!isSuccess)
                {
                    ModelState.AddModelError("", "Sth. went wrong!");
                    return View(model);
                }

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ModelState.AddModelError("", "Sth. went wrong!");
                return View(model);
            }
        }

        // GET: LeaveTypeController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            var isExist = await _repo.isExists(id);
            if (!isExist)
            {
                return NotFound();
            }
            var leaveType =await _repo.FindById(id);
            var model = _mapper.Map<DetailsLeaveTypeVM>(leaveType);

            return View(model);
        }

        // POST: LeaveTypeController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, DetailsLeaveTypeVM model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var leaveType = _mapper.Map<LeaveType>(model);
                

                var isSuccess =await _repo.Update(leaveType);
                if (!isSuccess)
                {
                    ModelState.AddModelError("", "Sth. went wrong!");
                    return View(model);
                }
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ModelState.AddModelError("", "Sth. went wrong!");
                return View(model);
            }
        }

        // GET: LeaveTypeController/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            var leaveType =await _repo.FindById(id);
            if (leaveType == null)
            {
                return NotFound();
            }

            var isSuccess =await _repo.Delete(leaveType);
            if (!isSuccess)
            {
                return BadRequest();
            }
            return RedirectToAction(nameof(Index));
            //if (!_repo.isExists(id))
            //{
            //    return NotFound();
            //}
            //var leaveType = _repo.FindById(id);
            //var model = _mapper.Map<DetailsLeaveTypeVM>(leaveType);

            //return View(model);
        }

        // POST: LeaveTypeController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id, DetailsLeaveTypeVM model)
        {
            try
            {                
                var leaveType =await _repo.FindById(id);
                if (leaveType==null)
                {
                    return NotFound();
                }
                
                var isSuccess =await _repo.Delete(leaveType);
                if (!isSuccess)
                {                   
                    return View(model);
                }
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View(model);
            }
        }
    }
}
