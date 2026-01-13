namespace ELibrary.WebApp.DTO.Request
{
    public class ReservationRequestDTO
    {
      

        public int ReaderId { get; set; }

        public int BookId { get; set; }

        public DateOnly ReservationDate { get; set; }

        public string? Status { get; set; }

        public DateOnly? PickupDate { get; set; }

        public int Quantity { get; set; }
    }
}
