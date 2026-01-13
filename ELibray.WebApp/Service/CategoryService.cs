using E_Library.Models;
using E_Library.Repository;

namespace ELibrary.WebApp.Service
{
    public class CategoryService : ICategoryService
    {

        private readonly ICategoryRepository _repository;

        public CategoryService(ICategoryRepository repository)
        {
            _repository = repository;
        }
        public Task<List<Category>> GetAll()
        {
            return _repository.GetAll();
        }
    }
}
