using E_Library.Models;
using E_Library.Repository;

namespace ELibrary.WebApp.Service
{
    public class PermissionService : IPermissionService
    {
        private readonly IPermissionRepository _permissionRepository;

        public PermissionService(IPermissionRepository permissionRepository)
        {
            _permissionRepository = permissionRepository;
        }

        public async Task<List<Permission>> GetPermissionsByRoleId(int roleId)
        {
            return await _permissionRepository.GetPermissionByRoleAsync(roleId);
        }

        // ✅ THÊM MỚI: Kiểm tra quyền truy cập URL
        public async Task<bool> HasPermission(int roleId, string url)
        {
            try
            {
                var permissions = await _permissionRepository.GetPermissionByRoleAsync(roleId);
                
                // Kiểm tra URL có trong danh sách permissions không
                return permissions.Any(p => 
                    !string.IsNullOrEmpty(p.Link) && 
                    url.StartsWith(p.Link.ToLowerInvariant(), StringComparison.OrdinalIgnoreCase));
            }
            catch
            {
                return false; // Nếu có lỗi thì từ chối quyền truy cập
            }
        }

        // ✅ THÊM MỚI: Lấy tất cả URL được phép truy cập
        public async Task<List<string>> GetAllowedUrls(int roleId)
        {
            try
            {
                var permissions = await _permissionRepository.GetPermissionByRoleAsync(roleId);
                return permissions
                    .Where(p => !string.IsNullOrEmpty(p.Link))
                    .Select(p => p.Link!)
                    .ToList();
            }
            catch
            {
                return new List<string>();
            }
        }

        // ✅ THÊM MỚI: Cache permissions trong session
        public async Task<List<string>> GetPermissionLinksForSession(int roleId)
        {
            return await GetAllowedUrls(roleId);
        }
    }
}
