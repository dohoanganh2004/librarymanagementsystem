using E_Library.Models;

namespace ELibrary.WebApp.Service
{
    public interface IPermissionService
    {
        Task<List<Permission>> GetPermissionsByRoleId(int roleId);
        
      
        Task<bool> HasPermission(int roleId, string url);
        
    
        Task<List<string>> GetAllowedUrls(int roleId);
        
    
        Task<List<string>> GetPermissionLinksForSession(int roleId);
    }
}
