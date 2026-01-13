# Script để xóa tất cả files do Kiro tạo trong phiên này
# Chạy với cẩn thận!

Write-Host "🗑️  CLEANUP SCRIPT - XÓA FILES DO KIRO TẠO" -ForegroundColor Yellow
Write-Host "⚠️  Cảnh báo: Script này sẽ xóa các files đã tạo!" -ForegroundColor Red
Write-Host ""

$filesToDelete = @(
    "Middlewares/AuthorizationMiddleware.cs",
    "Security/RequirePermissionAttribute.cs", 
    "Security/AuthorizationHelper.cs",
    "Extensions/HtmlHelperExtensions.cs",
    "SQL/SamplePermissions.sql",
    "Examples/ControllerAuthorizationExample.cs",
    "Models/PaginationModel.cs",
    "Views/Shared/_Pagination.cshtml",
    "Views/Shared/_BookPagination.cshtml", 
    "Views/Shared/_Sidebar.cshtml",
    "wwwroot/css/sidebar.css",
    "Views/Shared/_AuthLayout.cshtml",
    "fix-views.ps1",
    "SQL/AllSystemUrls_Permissions.sql",
    "cleanup-kiro-files.ps1"  # Tự xóa chính nó
)

$confirm = Read-Host "Bạn có chắc muốn xóa tất cả files này? (y/N)"

if ($confirm -eq 'y' -or $confirm -eq 'Y') {
    foreach ($file in $filesToDelete) {
        $fullPath = Join-Path $PSScriptRoot $file
        if (Test-Path $fullPath) {
            Remove-Item $fullPath -Force
            Write-Host "✅ Đã xóa: $file" -ForegroundColor Green
        } else {
            Write-Host "❌ Không tìm thấy: $file" -ForegroundColor Gray
        }
    }
    
    Write-Host ""
    Write-Host "🎉 Hoàn thành! Tất cả files do Kiro tạo đã được xóa." -ForegroundColor Green
    Write-Host "⚠️  Lưu ý: Các thay đổi trong files cũ vẫn còn, cần revert thủ công." -ForegroundColor Yellow
} else {
    Write-Host "❌ Hủy bỏ. Không có file nào bị xóa." -ForegroundColor Red
}

Read-Host "Nhấn Enter để đóng..."