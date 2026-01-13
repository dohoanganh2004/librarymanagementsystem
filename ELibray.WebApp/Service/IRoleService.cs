using E_Library.Models;

namespace ELibrary.WebApp.Service
{
    public interface IRoleService
    {
        Task<List<Role>> GetAll();
        
    }
}
