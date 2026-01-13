using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Library.Models;
using Microsoft.EntityFrameworkCore;

namespace E_Library.Repository
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly ElibraryContext _context;

        public EmployeeRepository(ElibraryContext context)
        {
            _context = context;
        }

        // create employee
        public async Task<Employee> CreateEmployee(Employee employee)
        {
            await _context.AddAsync(employee);
            await _context.SaveChangesAsync();
            return employee;
        }

        // delete employee
        public async Task DeleteEmployee(int? employeeId)
        {
            var employee = await GetById(employeeId);
            if (employee != null)
            {
                _context.Remove(employee);
                await _context.SaveChangesAsync();
            }
        }

        // get all employees
        public async Task<List<Employee>> GetAll(string? firstName, string? lastName, DateOnly? Dob,
                                                 string? email, string? phoneNumber, int? RoleID,
                                                 bool? status)
        {
            List<Employee> employees = await _context.Employees
                                                     .Include(e => e.Role)
                                                     .ToListAsync();

            if (firstName != null)
            {
                employees = employees
                    .Where(e => e.FirstName != null &&
                                e.FirstName.ToLower().Contains(firstName.ToLower()))
                    .ToList();
            }

            if (lastName != null)
            {
                employees = employees
                    .Where(e => e.LastName != null &&
                                e.LastName.ToLower().Contains(lastName.ToLower()))
                    .ToList();
            }

            if (Dob != null)
            {
                DateOnly target = Dob.Value;

                employees = employees
                    .Where(e => e.DoB.HasValue && e.DoB.Value == target)
                    .ToList();
            }

            if (email != null)
            {
                employees = employees
                    .Where(e => e.Email != null &&
                                e.Email.ToLower().Contains(email.ToLower()))
                    .ToList();
            }

            if (phoneNumber != null)
            {
                employees = employees
                    .Where(e => e.PhoneNumber != null &&
                                e.PhoneNumber.ToLower().Contains(phoneNumber.ToLower()))
                    .ToList();
            }

            if (RoleID != null)
            {
                employees = employees
                    .Where(e => e.RoleId == RoleID.Value)
                    .ToList();
            }

            if (status != null)
            {
                employees = employees
                    .Where(e => e.Status == status.Value)
                    .ToList();
            }

            return employees;
        }

        // get employee by email
        public async Task<Employee> GetByEmail(string email)
        {
            return await _context.Employees.Include(e => e.Role)
                                 .FirstOrDefaultAsync(e => e.Email.Equals(email));
        }

        // get employee by id
        public async Task<Employee> GetById(int? id)
        {
            return await _context.Employees
                                 .Include(e => e.Role)
                                 .FirstOrDefaultAsync(e => e.EmployeeId == id);
        }

        // get employee by phone number
        public async Task<Employee> GetByPhoneNumber(string phoneNumber)
        {
            return await _context.Employees
                                 .FirstOrDefaultAsync(e => e.PhoneNumber.Equals(phoneNumber));
        }

        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }

        // update employee
        public async Task UpdateEmployee(Employee employee)
        {
            _context.Update(employee);
            await _context.SaveChangesAsync();
        }
    }
}
