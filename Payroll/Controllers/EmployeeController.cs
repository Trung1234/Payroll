﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Payroll.Entity;
using Payroll.Services;
using Payroll.Controllers.Base;
using Payroll.Utility;
using Payroll.Models.EmployeeVM;

namespace Payroll.Controllers
{
    public class EmployeeController : BaseController
    {
        private readonly IEmployeeService _employeeService;
        private readonly HostingEnvironment _hostingEnvironment;

        public EmployeeController(IEmployeeService employeeService, 
            HostingEnvironment hostingEnvironment,
            ICompositeViewEngine viewEngine) : base(viewEngine)
        {
            _employeeService = employeeService;
            _hostingEnvironment = hostingEnvironment;
        }

        public override IActionResult Index()
        {
            ViewBag.ErrorMessage = "";
            List<EmployeeDisplayViewModel> employees;
            try
            {
                employees = _employeeService.GetAll().Select(employee => new EmployeeDisplayViewModel
                {
                    Id = employee.Id,
                    City = employee.City,
                    DateJoined = employee.DateJoined,
                    Designation = employee.Designation,
                    EmployeeNo = employee.EmployeeNo,
                    FullName = employee.FullName,
                    Gender = employee.Gender,
                    ImageUrl = employee.ImageUrl
                }).ToList();
            }catch(Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                employees = new List<EmployeeDisplayViewModel>();
            }           
            return View(employees);
        }

        [HttpGet]
        public override IActionResult Create()
        {
            return View(new EmployeeCreateViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]// Prevent cross-site Request Forgery Attacks
        public async Task<IActionResult> Create(EmployeeCreateViewModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.EmployeeNo))
            {
                IEnumerable<Employee> employees = _employeeService.GetAll();
                if (employees.Any(s => s.EmployeeNo.Equals(model.EmployeeNo)))
                {
                    ModelState.AddModelError(nameof(model.EmployeeNo), "Employee No is duplicated");
                    return View();
                }
            }
            if (ModelState.IsValid)
            {                                                
                var employee = new Employee
                {
                    Id = model.Id,
                    EmployeeNo = model.EmployeeNo,
                    FirstName = model.FirstName,
                    MiddleName = model.MiddleName,
                    LastName = model.LastName,
                    FullName = model.FullName,
                    Gender = model.Gender,
                    Email = model.Email,
                    DOB = model.DOB,
                    DateJoined = model.DateJoined,
                    NationalInsuranceNo = model.NationalInsuranceNo,
                    PaymentMethod = model.PaymentMethod,
                    StudentLoan = model.StudentLoan,
                    UnionMember = model.UnionMember,
                    Address = model.Address,
                    City = model.City,
                    Phone = model.Phone,
                    Postcode = model.Postcode,
                    Designation = model.Designation,
                };
                if (model.ImageUrl != null && model.ImageUrl.Length > 0)
                {
                    string uploadDir = @"images/employee";
                    string fileName = Path.GetFileNameWithoutExtension(model.ImageUrl.FileName);
                    string extension = Path.GetExtension(model.ImageUrl.FileName);
                    string webRootPath = _hostingEnvironment.WebRootPath;
                    fileName = DateTime.UtcNow.ToString("yymmssfff") + fileName + extension;
                    employee.ImageUrl = "/" + uploadDir + "/" + fileName;
                    string path = Path.Combine(webRootPath, uploadDir, fileName);
                    await model.ImageUrl.CopyToAsync(new FileStream(path, FileMode.Create));                  
                }
                await _employeeService.CreateAsync(employee);
                return Json(new { isValid = true , message = Constants.CreateEmployeeSuccess});
            }
            return Json(new { isValid = false, errors = GetErrorsFromModelState() });
        }

        public IActionResult Edit(int id)
        {
            var employee = _employeeService.GetById(id);
            if (employee == null)
            {
                return NotFound();
            }
            var model = new EmployeeEditViewModel()
            {
                Id = employee.Id,
                EmployeeNo = employee.EmployeeNo,
                FirstName = employee.FirstName,
                MiddleName = employee.MiddleName,
                LastName = employee.LastName,
                Gender = employee.Gender,
                Email = employee.Email,
                DOB = employee.DOB,
                DateJoined = employee.DateJoined,
                NationalInsuranceNo = employee.NationalInsuranceNo,
                PaymentMethod = employee.PaymentMethod,
                StudentLoan = employee.StudentLoan,
                UnionMember = employee.UnionMember,
                Address = employee.Address,
                City = employee.City,
                Phone = employee.Phone,
                Postcode = employee.Postcode,
                Designation = employee.Designation,
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EmployeeEditViewModel model)
        {
            if (ModelState.IsValid)
            {               
                var employee = _employeeService.GetById(model.Id);
                if (employee == null)
                {
                    return NotFound();
                }
                employee.EmployeeNo = model.EmployeeNo;
                employee.FirstName = model.FirstName;
                employee.LastName = model.LastName;
                employee.MiddleName = model.MiddleName;
                employee.NationalInsuranceNo = model.NationalInsuranceNo;
                employee.Gender = model.Gender;
                employee.Email = model.Email;
                employee.DOB = model.DOB;
                employee.DateJoined = model.DateJoined;
                employee.Phone = model.Phone;
                employee.Designation = model.Designation;
                employee.PaymentMethod = model.PaymentMethod;
                employee.StudentLoan = model.StudentLoan;
                employee.UnionMember = model.UnionMember;
                employee.Address = model.Address;
                employee.City = model.City;
                employee.Postcode = model.Postcode;
                if (model.ImageUrl != null && model.ImageUrl.Length > 0)
                {
                    string uploadDir = @"images/employee";
                    string fileName = Path.GetFileNameWithoutExtension(model.ImageUrl.FileName);
                    string extension = Path.GetExtension(model.ImageUrl.FileName);
                    string webRootPath = _hostingEnvironment.WebRootPath;
                    fileName = DateTime.UtcNow.ToString("yymmssfff") + fileName + extension;
                    employee.ImageUrl = "/" + uploadDir + "/" + fileName;
                    string path = Path.Combine(webRootPath, uploadDir, fileName);
                    await model.ImageUrl.CopyToAsync(new FileStream(path, FileMode.Create));
                }
                await _employeeService.UpdateAsync(employee);
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        [HttpGet]
        public override IActionResult Detail(int id)
        {
            var employee = _employeeService.GetById(id);
            if (employee == null)
            {
                return NotFound();
            }
            EmployeeDisplayViewModel model = new EmployeeDisplayViewModel()
            {
                Id = employee.Id,
                EmployeeNo = employee.EmployeeNo,
                FullName = employee.FullName,
                Gender = employee.Gender,
                DOB = employee.DOB,
                DateJoined = employee.DateJoined,
                Designation = employee.Designation,
                NationalInsuranceNo = employee.NationalInsuranceNo,
                Phone = employee.Phone,
                Email = employee.Email,
                PaymentMethod = employee.PaymentMethod,
                StudentLoan = employee.StudentLoan,
                UnionMember = employee.UnionMember,
                Address = employee.Address,
                City = employee.City,
                ImageUrl = employee.ImageUrl,
                Postcode = employee.Postcode
            };
            return View(model);
        }

        [HttpGet]
        public IActionResult ShowDeletePopup(int id)
        {
            var employee = _employeeService.GetById(id);
            if (employee == null)
            {
                return NotFound();
            }
            var model = new EmployeeDeleteViewModel()
            {
                Id = employee.Id,
                EmployeeNo = employee.EmployeeNo,
                FullName = employee.FullName
            };
            return Json(new {model = model, view = RenderPartialViewToString("_Delete") });
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<JsonResult> Delete(string id)
        //{
        //    await _employeeService.DeleteAsync(Convert.ToInt32(id));
        //    return Json(new {  message = Constants.DeleteEmployeeSuccess });
        //}

        [HttpPost]
        public async Task<JsonResult> DeleteEmployee(EmployeeDeleteViewModel model)
        {
            await _employeeService.DeleteAsync(model.Id);
            return Json(new { message = Constants.DeleteEmployeeSuccess });
        }
    }
}
