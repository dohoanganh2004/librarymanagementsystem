using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Library.Models;
using Microsoft.EntityFrameworkCore;

namespace E_Library.Repository
{
    public class RoleRepository : IRoleRepository
    {
        private readonly ElibraryContext _context;
        public RoleRepository(ElibraryContext context)
        {
            _context = context;
        }
        public Task<List<Role>> GetAll()
        {
            return _context.Roles.ToListAsync();   
        }
    }
}
