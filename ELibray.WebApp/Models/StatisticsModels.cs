namespace ELibrary.WebApp.Models
{
    /// <summary>
    /// Dashboard tổng quan
    /// </summary>
    public class DashboardViewModel
    {
        public int TotalBooks { get; set; }
        public int TotalReaders { get; set; }
        public int TotalEmployees { get; set; }
        public int ActiveCheckouts { get; set; }
        public int PendingReservations { get; set; }
        public int OverdueBooks { get; set; }
        
        // Thống kê theo thời gian
        public List<ChartDataPoint> CheckoutTrend { get; set; } = new();
        public List<ChartDataPoint> RegistrationTrend { get; set; } = new();
        
        // Top sách
        public List<TopBookItem> TopBooks { get; set; } = new();
        
        // Thống kê theo danh mục
        public List<CategoryStatItem> CategoryStats { get; set; } = new();
    }

    /// <summary>
    /// Thống kê sách chi tiết
    /// </summary>
    public class BookStatisticsViewModel
    {
        public int TotalBooks { get; set; }
        public int BooksAdded { get; set; }
        public int BooksUpdated { get; set; }
        public int BooksDeleted { get; set; }
        public int MostBorrowedCount { get; set; }
        public string MostBorrowedBook { get; set; } = "";
        
        public List<TopBookItem> TopBorrowedBooks { get; set; } = new();
        public List<CategoryStatItem> BooksByCategory { get; set; } = new();
        public List<ChartDataPoint> BorrowingTrend { get; set; } = new();
        public List<AuthorStatItem> TopAuthors { get; set; } = new();
    }

    /// <summary>
    /// Thống kê người dùng
    /// </summary>
    public class UserStatisticsViewModel
    {
        public int TotalReaders { get; set; }
        public int NewRegistrations { get; set; }
        public int ActiveUsers { get; set; }
        public int InactiveUsers { get; set; }
        
        public List<ChartDataPoint> RegistrationTrend { get; set; } = new();
        public List<AgeGroupStatItem> UsersByAge { get; set; } = new();
        public List<TopReaderItem> TopReaders { get; set; } = new();
    }

    /// <summary>
    /// Điểm dữ liệu cho biểu đồ
    /// </summary>
    public class ChartDataPoint
    {
        public string Label { get; set; } = "";
        public int Value { get; set; }
        public DateTime Date { get; set; }
    }

    /// <summary>
    /// Top sách được mượn
    /// </summary>
    public class TopBookItem
    {
        public int BookId { get; set; }
        public string Title { get; set; } = "";
        public string Author { get; set; } = "";
        public string Category { get; set; } = "";
        public int BorrowCount { get; set; }
        public string Image { get; set; } = "";
    }

    /// <summary>
    /// Thống kê theo danh mục
    /// </summary>
    public class CategoryStatItem
    {
        public string CategoryName { get; set; } = "";
        public int BookCount { get; set; }
        public int BorrowCount { get; set; }
        public double Percentage { get; set; }
    }

    /// <summary>
    /// Thống kê tác giả
    /// </summary>
    public class AuthorStatItem
    {
        public string AuthorName { get; set; } = "";
        public int BookCount { get; set; }
        public int TotalBorrows { get; set; }
    }

    /// <summary>
    /// Thống kê độ tuổi
    /// </summary>
    public class AgeGroupStatItem
    {
        public string AgeGroup { get; set; } = "";
        public int UserCount { get; set; }
        public double Percentage { get; set; }
    }

    /// <summary>
    /// Top độc giả
    /// </summary>
    public class TopReaderItem
    {
        public int ReaderId { get; set; }
        public string FullName { get; set; } = "";
        public string Email { get; set; } = "";
        public int BorrowCount { get; set; }
        public DateTime LastBorrow { get; set; }
    }

    /// <summary>
    /// Tham số lọc thống kê
    /// </summary>
    public class StatisticsFilter
    {
        public DateTime FromDate { get; set; } = DateTime.Now.AddMonths(-1);
        public DateTime ToDate { get; set; } = DateTime.Now;
        public string Period { get; set; } = "month"; 
        public int? CategoryId { get; set; }
        public int? AuthorId { get; set; }
    }
}