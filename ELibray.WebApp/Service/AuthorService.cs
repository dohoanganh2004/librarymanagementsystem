using E_Library.Models;
using E_Library.Repository;

namespace ELibrary.WebApp.Service
{
    public class AuthorService : IAuthorService
    {
        private readonly IAuthorRepository authorRepository;

        public AuthorService(IAuthorRepository authorRepository)
        {
            this.authorRepository = authorRepository;
        }
        public Task<List<Author>> GetAll()
        {
            return authorRepository.GetAll();
        }
    }
}
