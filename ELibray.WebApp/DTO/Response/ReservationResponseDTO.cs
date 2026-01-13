namespace ELibrary.WebApp.DTO.Response
{
    public class ReservationResponseDTO
    {
        public int ReservationId { get; set; }

        public int ReaderId { get; set; }
        public string ReaderName { get; set; } = null!;

      
        public int BookId { get; set; }
        public string Title { get; set; } = null!;

        public DateOnly ReservationDate { get; set; }

        public string? Status { get; set; }

        public DateOnly? PickupDate { get; set; }
        public int Quantity { get; set; }
    }
}
