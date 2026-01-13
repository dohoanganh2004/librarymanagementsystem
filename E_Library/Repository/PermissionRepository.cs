using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Library.Models;
using Microsoft.EntityFrameworkCore;

namespace E_Library.Repository
{
    public class PermissionRepository : IPermissionRepository
    {
     
        public readonly ElibraryContext _context;

        public PermissionRepository(ElibraryContext context)
        {
            _context = context;
        }

        public async Task<Permission> GetByIdAsync(int id)
        {
              return await _context.Permissions.FindAsync(id);
            
        }

        public async Task<List<Permission>> GetPermissionByRoleAsync(int? roleId)
        {
            return await _context.Roles
            .Where(r => r.RoleId == roleId)
            .SelectMany(r => r.Permissions)
            .ToListAsync();
        }
    }
        
}
