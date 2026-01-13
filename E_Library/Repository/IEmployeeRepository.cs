using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Library.Models;

namespace E_Library.Repository
{
    public interface IEmployeeRepository
    {
        Task<List<Employee>> GetAll(string? firstName, string? lastName, DateOnly? Dob,
                                      string? email, string? phoneNumber, int? RoleID,
                                      bool? status);

        Task<Employee?> GetById(int? id);

        Task<Employee?> GetByEmail(string email);

        Task<Employee?> GetByPhoneNumber(string phoneNumber);

        Task<Employee> CreateEmployee(Employee employee);

        Task UpdateEmployee(Employee employee);

        Task DeleteEmployee(int? employeeId);

        Task Save();
    }
}
