using E_Library.Models;
using ELibrary.WebApp.DTO.Request;
using ELibrary.WebApp.DTO.Response;

namespace ELibrary.WebApp.Service
{
    public interface IReservationService
    {
        Task<List<ReservationResponseDTO>> GetReservationsByReader(int? readerID);
        Task<List<ReservationResponseDTO>> GetAllReservations();
        Task<ReservationResponseDTO> GetReservationById(int? readerId ,int? reservationID);
        Task<Reservation> CreateReservation(ReservationRequestDTO reservationRequestDTO);
        Task<bool> UpdateStatus(int id, string actionType);

        Task<bool> UpdateReservation(int? id, ReservationRequestDTO reservationRequestDto);

        Task<bool> DeleteReservation(int reservationID);
    }
}
