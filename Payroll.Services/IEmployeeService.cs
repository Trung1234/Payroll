﻿using Microsoft.AspNetCore.Mvc.Rendering;
using Payroll.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Payroll.Services
{
    public interface IEmployeeService
    {
        Task CreateAsync(Employee newEmployee);
        Employee GetById(int employeeId);
        Task UpdateAsync(Employee employee);
        Task UpdateAsync(int id);
        Task DeleteAsync(int employeeId);
        decimal StudentLoanRepaymentAmount(int id, decimal totalAmount);
        IEnumerable<Employee> GetAll();
        IEnumerable<SelectListItem> GetAllEmployeesForPayroll();
        decimal UnionFees(int employeeId);
    }
}
