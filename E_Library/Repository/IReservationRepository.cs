using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Library.Models;

namespace E_Library.Repository
{
    public interface IReservationRepository
    {
       Task<List<Reservation>> GetReservationsByReaderID(int? readerID);
       Task<List<Reservation>> GetAllReservations();
        Task<Reservation> GetReservation(int? reservationID);
       Task<Reservation> CreateNewReservation(Reservation reservation);
       Task<bool> UpdateReservation (Reservation reservation);
        Task DeleteReservation(int? reservationID);


    }
}
