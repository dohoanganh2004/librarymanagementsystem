using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Library.Models;
using Microsoft.EntityFrameworkCore;

namespace E_Library.Repository
{
    public class AuthorRepository : IAuthorRepository
    {
        private readonly ElibraryContext _context;

        public  AuthorRepository(ElibraryContext context)
        {
            _context = context;
        }

        public async Task<List<Author>> GetAll()
        {
            return await _context.Authors.ToListAsync();
        }
    }
}
