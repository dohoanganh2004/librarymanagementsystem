using AutoMapper;
using E_Library.Models;
using E_Library.Repository;
using ELibrary.WebApp.DTO.Request;
using ELibrary.WebApp.DTO.Response;
using ELibrary.WebApp.EmailServices;
using ELibrary.WebApp.Hubs;
using ELibrary.WebApp.Models;
using Microsoft.AspNetCore.SignalR;

namespace ELibrary.WebApp.Service
{
    public class ReservationService : IReservationService
    {
        private readonly IReaderService _readerService;
        private readonly IReservationRepository _reservationRepository;
        private readonly IMapper _mapper;
        private readonly IBookService _bookService;
        private readonly ICheckoutRepository _checkoutRepository;
        private readonly IHubContext<NotificationHub> _hub;
        private readonly INotificationService _notificationService;

        public ReservationService(
            IReaderService readerService,
            IReservationRepository reservationRepository,
            IMapper mapper,
            IBookService bookService,
            ICheckoutRepository checkoutRepository,
            IHubContext<NotificationHub> hub,
            INotificationService notificationService
            )
        {
            _readerService = readerService;
            _reservationRepository = reservationRepository;
            _mapper = mapper;
            _bookService = bookService;
            _checkoutRepository = checkoutRepository;
            _hub = hub;
            _notificationService = notificationService;
        }

        // create new reservation (async)
        public async Task<Reservation> CreateReservation(ReservationRequestDTO reservationRequestDTO)
        {
            var book = await _bookService.GetBookByID(reservationRequestDTO.BookId);
            if (book == null) throw new Exception($"Không tìm thấy sách với ID = {reservationRequestDTO.BookId}");

            var reservation = new Reservation
            {
                BookId = reservationRequestDTO.BookId,
                ReaderId = reservationRequestDTO.ReaderId,
                ReservationDate = reservationRequestDTO.ReservationDate,
                Status = "Pending",
                PickupDate = reservationRequestDTO.PickupDate
            };
           

            book.Quantity -= reservationRequestDTO.Quantity;

            await _reservationRepository.CreateNewReservation(reservation);
            
            // Notify reader about successful reservation
            await _hub.Clients.Group($"reader-{reservation.ReaderId}")
                       .SendAsync(
                           "ReceiveMessage",
                           "ELibrary",
                           "Bạn đã đặt sách thành công hãy chờ quản lý phê duyệt!"
                       );
            
            // Notify librarians about new reservation
            await _hub.Clients.Group("role-2")
                        .SendAsync(
                            "ReceiveMessage",
                            "ELibrary",
                            $"Có đơn đặt sách mới từ ReaderId: {reservation.ReaderId}"
                        );
            
            // Notify admins about new reservation
            await _hub.Clients.Group("role-1")
                       .SendAsync(
                           "ReceiveMessage",
                           "ELibrary",
                           $"Có đơn đặt sách mới từ ReaderId: {reservation.ReaderId}"
                       );

            return reservation;
        }

        // get reservation by id (async)
        public async Task<ReservationResponseDTO> GetReservationById(int? readerId, int? reservationID)
        {
            // If readerId is provided, validate the reader exists
            if (readerId != null)
            {
                var reader = await _readerService.getReaderByID(readerId);
                if (reader == null)
                    throw new Exception("Không tìm thấy độc giả với ID = " + readerId);
            }

            var reservation = await _reservationRepository.GetReservation(reservationID);
            if (reservation == null)
                throw new Exception("Không tìm thấy reservation với ID = " + reservationID);

            // If readerId is provided, check if reservation belongs to that reader
            if (readerId != null && reservation.ReaderId != readerId)
                throw new Exception("Bạn không có quyền xem reservation này");

            return _mapper.Map<ReservationResponseDTO>(reservation);
        }

        // get list reservation of reader (async)
        public async Task<List<ReservationResponseDTO>> GetReservationsByReader(int? readerID)
        {
            var reader = await _readerService.getReaderByID(readerID);
            if (reader == null)
                throw new Exception("Không tìm thấy độc giả với ID = " + readerID);

            var reservations = await _reservationRepository.GetReservationsByReaderID(readerID);
            return _mapper.Map<List<ReservationResponseDTO>>(reservations);
        }

        // get all reservations (async) - for admin/librarian
        public async Task<List<ReservationResponseDTO>> GetAllReservations()
        {
            var reservations = await _reservationRepository.GetAllReservations();
            return _mapper.Map<List<ReservationResponseDTO>>(reservations);
        }

        // update status (async)
        public async Task<bool> UpdateStatus(int id, string actionType)
        {
            var reservation = await _reservationRepository.GetReservation(id);
            if (reservation == null)
                return false;

            switch (actionType.ToLower())
            {
                case "approve":
                    reservation.Status = "Approved";
                    reservation.PickupDate = DateOnly.FromDateTime(DateTime.Now);

                    var dto = new CheckoutRequestDTO
                    {
                        BookId = reservation.BookId,
                        ReaderId = reservation.ReaderId,
                        BrowseDate = DateOnly.FromDateTime(DateTime.Now),
                        DueDate = DateOnly.FromDateTime(DateTime.Now.AddDays(7)),
                        Status = "PendingPickup"
                    };

                    var checkout = _mapper.Map<Checkout>(dto);
                    await _checkoutRepository.NewCheckout(checkout);

                    // Notify reader
                    await _hub.Clients.Group($"reader-{reservation.ReaderId}")
                        .SendAsync(
                            "ReceiveMessage",
                            "ELibrary",
                            "Yêu cầu đặt sách của bạn đã được PHÊ DUYỆT"
                        );
                    
                    // Notify staff (librarians and admins)
                    await _hub.Clients.Group("role-2")
                        .SendAsync(
                            "ReceiveMessage",
                            "ELibrary",
                            $"Đã phê duyệt đơn đặt sách của Reader #{reservation.ReaderId}"
                        );
                    await _hub.Clients.Group("role-1")
                        .SendAsync(
                            "ReceiveMessage",
                            "ELibrary",
                            $"Đã phê duyệt đơn đặt sách của Reader #{reservation.ReaderId}"
                        );
                    break;

                case "reject":
                    reservation.Status = "Rejected";

                    // Notify reader
                    await _hub.Clients.Group($"reader-{reservation.ReaderId}")
                        .SendAsync(
                            "ReceiveMessage",
                            "ELibrary",
                            "Yêu cầu đặt sách của bạn đã bị TỪ CHỐI"
                        );
                    
                    // Notify staff (librarians and admins)
                    await _hub.Clients.Group("role-2")
                        .SendAsync(
                            "ReceiveMessage",
                            "ELibrary",
                            $"Đã từ chối đơn đặt sách của Reader #{reservation.ReaderId}"
                        );
                    await _hub.Clients.Group("role-1")
                        .SendAsync(
                            "ReceiveMessage",
                            "ELibrary",
                            $"Đã từ chối đơn đặt sách của Reader #{reservation.ReaderId}"
                        );
                    break;

                case "cancel":
                    reservation.Status = "Canceled";

                    await _hub.Clients.Group($"reader-{reservation.ReaderId}")
                       .SendAsync(
                           "ReceiveMessage",
                           "ELibrary",
                           "Đơn đặt sách của bạn đã được hủy"
                       );
                    
                    // Notify staff (librarians and admins)
                    await _hub.Clients.Group("role-2") 
                        .SendAsync(
                            "ReceiveMessage",
                            "ELibrary",
                            $"Đơn đặt sách của Reader #{reservation.ReaderId} đã bị hủy"
                        );
                    await _hub.Clients.Group("role-1") 
                        .SendAsync(
                            "ReceiveMessage",
                            "ELibrary",
                            $"Đơn đặt sách của Reader #{reservation.ReaderId} đã bị hủy"
                        );
                    break;

                default:
                    return false;
            }

            await _reservationRepository.UpdateReservation(reservation);
            return true;
        }


        // update reservation (async)
        public async Task<bool> UpdateReservation(int? id, ReservationRequestDTO reservationRequestDto)
        {
            var reservation = await _reservationRepository.GetReservation(id);
            if (reservation == null || reservation.Status == "Pending")
            {
                return false;
            }

            var updateReservation = _mapper.Map<Reservation>(reservationRequestDto);
            await _reservationRepository.UpdateReservation(updateReservation);
            return true;
        }

        // delete reservation if status = canceled (async)
        public async Task<bool> DeleteReservation(int reservationID)
        {
            var reservation = await _reservationRepository.GetReservation(reservationID);
            if (reservation == null || reservation.Status != "Canceled")
            {
                return false;
            }
            await _hub.Clients.Group($"reader-{reservation.ReaderId}")
                       .SendAsync(
                           "ReceiveMessage",
                           "ELibrary",
                           "Bạn đã xóa một phiếu mượn sách!"
                       );
            await _reservationRepository.DeleteReservation(reservationID);
            return true;
        }
    }
}
