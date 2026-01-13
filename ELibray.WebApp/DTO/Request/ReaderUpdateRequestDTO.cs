namespace ELibrary.WebApp.DTO.Request
{
    public class ReaderUpdateRequestDTO
    {
        public int ReaderId { get; set; } 

        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public DateOnly? DoB { get; set; }

        public string? Address { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Avatar { get; set; }
    }
}
