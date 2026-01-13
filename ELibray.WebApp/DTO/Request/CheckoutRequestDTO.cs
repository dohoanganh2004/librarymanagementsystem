namespace ELibrary.WebApp.DTO.Request
{
    public class CheckoutRequestDTO
    {
        public int CheckoutId { get; set; }

        public int BookId { get; set; }

        public int ReaderId { get; set; }

        public DateOnly BrowseDate { get; set; }

        public DateOnly? ReturnDate { get; set; }

        public DateOnly? DueDate { get; set; }

        public string Status { get; set; } = null!;

    }
}
