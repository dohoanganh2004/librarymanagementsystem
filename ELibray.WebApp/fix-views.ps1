# Script để sửa tất cả Views bỏ _Header và _Footer

$viewFiles = @(
    "Views/Reservation/Create.cshtml",
    "Views/Employee/All.cshtml", 
    "Views/Reservation/Details.cshtml",
    "Views/Profile/EditProfile.cshtml",
    "Views/Home/Index.cshtml",
    "Views/Book/BlankPage.cshtml",
    "Views/Checkout/All.cshtml",
    "Views/Reservation/List.cshtml",
    "Views/Profile/ReaderProfile.cshtml",
    "Views/Book/Detail.cshtml",
    "Views/Employee/Create.cshtml",
    "Views/Employee/Details.cshtml",
    "Views/Employee/Update.cshtml"
)

foreach ($file in $viewFiles) {
    $fullPath = Join-Path $PSScriptRoot $file
    if (Test-Path $fullPath) {
        Write-Host "Processing: $file"
        
        $content = Get-Content $fullPath -Raw
        
        # Remove _Header
        $content = $content -replace '@await Html\.PartialAsync\("_Header"\)\s*', ''
        
        # Remove _Footer  
        $content = $content -replace '@await Html\.PartialAsync\("_Footer"\)\s*', ''
        
        # Replace container mt-4 with container-fluid
        $content = $content -replace '<div class="container mt-4">', '<div class="container-fluid">'
        
        Set-Content $fullPath $content -NoNewline
        Write-Host "Fixed: $file"
    }
}

Write-Host "All views fixed!"