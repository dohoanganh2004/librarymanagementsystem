using E_Library.Models;

namespace ELibrary.WebApp.Service
{
    public interface ICategoryService
    {
       Task<List<Category>> GetAll();
    }
}
