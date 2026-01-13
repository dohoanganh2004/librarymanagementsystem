namespace ELibrary.WebApp.DTO.Response
{
    public class ReaderResponseDTO
    {
        public int ReaderId { get; set; }

        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public DateOnly? DoB { get; set; }

        public int? Age { get; set; }

        public string? Address { get; set; }

        public string Email { get; set; } = null!;

        public string? PhoneNumber { get; set; }
        public string? Avatar { get; set; }
        public bool? Status { get; set; }

    }
}
