using E_Library.Models;
using E_Library.Repository;

namespace ELibrary.WebApp.Service
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;


        public RoleService(IRoleRepository roleRepository) 
        { 
            _roleRepository = roleRepository;
        }
        public Task<List<Role>> GetAll()
        {
            return _roleRepository.GetAll();  
        }
    }
}
