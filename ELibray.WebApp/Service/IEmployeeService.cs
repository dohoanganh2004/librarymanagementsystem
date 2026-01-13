using ELibrary.WebApp.DTO.Request;
using ELibrary.WebApp.DTO.Response;

namespace ELibrary.WebApp.Service
{
    public interface IEmployeeService
    {
        Task<List<EmployeeResponseDTO>> GetAll(
       string? firstName,
       string? lastName,
       DateOnly? Dob,
       string? email,
       string? phoneNumber,
       int? RoleID,
       bool? status);

        Task<EmployeeResponseDTO> GetByID(int? id);

        Task<EmployeeResponseDTO> Create(EmployeeRequestDTO employeeRequestDTO);

        Task<EmployeeResponseDTO> Update(EmployeeRequestDTO employeeRequestDTO);

        Task Delete(int? id);


        Task<bool> ChangeStatus(int? employeeId, string? actionType);

    }
}
