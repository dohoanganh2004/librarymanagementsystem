namespace ELibrary.WebApp.Excel
{
    public interface IBookExcelService
    {
        Task<byte[]> ExportBooksToExcelAsync();
        Task ImportBooksFromExcelAsync(IFormFile excelFile);
    }
}
