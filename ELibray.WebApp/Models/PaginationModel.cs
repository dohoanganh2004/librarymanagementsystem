namespace ELibrary.WebApp.Models
{
    /// <summary>
    /// Interface cho pagination để sử dụng trong Partial View
    /// </summary>
    public interface IPaginationModel
    {
        int CurrentPage { get; set; }
        int TotalPages { get; set; }
        int TotalItems { get; set; }
        int PageSize { get; set; }
        bool HasPreviousPage { get; }
        bool HasNextPage { get; }
        int StartItem { get; }
        int EndItem { get; }
        List<int> GetPageNumbers(int maxPagesToShow = 5);
    }

    /// <summary>
    /// Model cho phân trang
    /// </summary>
    public class PaginationModel<T> : IPaginationModel
    {
        public List<T> Items { get; set; } = new List<T>();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }
        public int PageSize { get; set; }
        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;
        
       
        public int StartItem => (CurrentPage - 1) * PageSize + 1;
        public int EndItem => Math.Min(CurrentPage * PageSize, TotalItems);
        
       
        public List<int> GetPageNumbers(int maxPagesToShow = 5)
        {
            var pages = new List<int>();
            
            if (TotalPages <= maxPagesToShow)
            {
               
                for (int i = 1; i <= TotalPages; i++)
                {
                    pages.Add(i);
                }
            }
            else
            {
                
                int startPage = Math.Max(1, CurrentPage - maxPagesToShow / 2);
                int endPage = Math.Min(TotalPages, startPage + maxPagesToShow - 1);
                
               
                if (endPage - startPage + 1 < maxPagesToShow)
                {
                    startPage = Math.Max(1, endPage - maxPagesToShow + 1);
                }
                
                for (int i = startPage; i <= endPage; i++)
                {
                    pages.Add(i);
                }
            }
            
            return pages;
        }
    }
    
    /// <summary>
    /// Parameters cho phân trang và tìm kiếm
    /// </summary>
    public class BookSearchParameters
    {
        public string? Title { get; set; }
        public int? PublisherId { get; set; }
        public int? CategoryId { get; set; }
        public string? Author { get; set; }
        public string? Language { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        



      
        // Validation
        public void Validate()
        {
            if (Page < 1) Page = 1;
            if (PageSize < 1) PageSize = 10;
            if (PageSize > 100) PageSize = 100; 
        }



    }
}