using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Library.Models;
using Microsoft.EntityFrameworkCore;

namespace E_Library.Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ElibraryContext _context;

        public CategoryRepository(ElibraryContext context)
        {
            _context = context;
        }

         public Task<List<Category>> GetAll()
        {
            return _context.Categories.ToListAsync();
        }
    }
}
