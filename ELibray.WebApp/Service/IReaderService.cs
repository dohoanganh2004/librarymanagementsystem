using E_Library.Models;
using ELibrary.WebApp.DTO.Request;
using ELibrary.WebApp.DTO.Response;
using Microsoft.AspNetCore.WebUtilities;

namespace ELibrary.WebApp.Service
{
    public interface IReaderService
    {
        
        Task<ReaderResponseDTO> getReaderByID(int ?readerId); 
        Task UpdateReader(ReaderUpdateRequestDTO readerUpdateRequestDTO);

        Task<List<ReaderResponseDTO>> getAll(string? firstName, string? lastName,
                              string? email, string? phoneNumber,
                              bool? status);

        Task<bool> changeStatus(int? readerId, string activityType);

        
        
    }
}
