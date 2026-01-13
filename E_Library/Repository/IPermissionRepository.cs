using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Library.Models;

namespace E_Library.Repository
{
    public interface IPermissionRepository
    {
       

        Task<List<Permission>> GetPermissionByRoleAsync(int? roleId);
        Task<Permission?> GetByIdAsync(int id);
        

    }
}
