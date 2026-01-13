namespace ELibrary.WebApp.DTO.Response
{
    public class BookResponseDTO
    {
        public int BookId { get; set; }

        public string Title { get; set; } = null!;

        public int? PublicationYear { get; set; }

        public int? PublisherId { get; set; }
        public string PublisherName { get; set; } = null!;
        public int? CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;
        public int? AuthorId { get; set; }
        public string AuthorName { get; set; } = null!;
        public string? Image { get; set; }

        public string? Description { get; set; }

        public int? PageCount { get; set; }

        public string? Language { get; set; }

        public int? Quantity { get; set; }

        public bool? Status { get; set; }

    }
}
