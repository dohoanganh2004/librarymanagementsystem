using E_Library.Models;
using Microsoft.EntityFrameworkCore;

namespace E_Library.Repository
{
    public class ReservationRepository : IReservationRepository
    {
        private readonly ElibraryContext _context;

        public ReservationRepository(ElibraryContext context)
        {
            _context = context;
        }

        // Create
        public async Task<Reservation> CreateNewReservation(Reservation reservation)
        {
            var newReservation = await _context.Reservations.AddAsync(reservation);
            await _context.SaveChangesAsync();
            return newReservation.Entity;
        }

        // Delete
        public async Task DeleteReservation(int? reservationID)
        {
            var reservation = await GetReservation(reservationID);
            if (reservation != null)
            {
                _context.Reservations.Remove(reservation);
                await _context.SaveChangesAsync();
            }
        }

        // Get all
        public async Task<List<Reservation>> GetAllReservations()
        {
            return await _context.Reservations
                .Include(r => r.Reader)
                .Include(r => r.Book)
                .ToListAsync();
        }

        // Get by ID
        public async Task<Reservation> GetReservation(int? reservationID)
        {
            return await _context.Reservations
                .Include(r => r.Reader)
                .Include(r => r.Book)
                .FirstOrDefaultAsync(r => r.ReservationId == reservationID);
        }

        // Get list by readerID
        public async Task<List<Reservation>> GetReservationsByReaderID(int? readerID)
        {
            return await _context.Reservations
                .Include(r => r.Reader)
                .Include(r => r.Book)
                .Where(r => r.ReaderId == readerID)
                .ToListAsync();
        }

        // Update
        public async Task<bool> UpdateReservation(Reservation reservation)
        {
            try
            {
                _context.Reservations.Update(reservation);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
