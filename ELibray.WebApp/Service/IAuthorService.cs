using E_Library.Models;

namespace ELibrary.WebApp.Service
{
    public interface IAuthorService
    {
        Task<List<Author>> GetAll();  
    }
}
