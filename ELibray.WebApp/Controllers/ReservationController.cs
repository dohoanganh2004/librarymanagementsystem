using Azure.Core;
using ELibrary.WebApp.DTO.Request;
using ELibrary.WebApp.DTO.Response;
using ELibrary.WebApp.Service;
using Microsoft.AspNetCore.Mvc;

namespace ELibrary.WebApp.Controllers
{
    public class ReservationController : Controller
    {
        private readonly IReservationService reservationService;
        private readonly IBookService bookService;
        private readonly IReaderService readerService;

        public ReservationController(
            IReservationService reservationService,
            IBookService bookService,
            IReaderService readerService)
        {
            this.reservationService = reservationService;
            this.bookService = bookService;
            this.readerService = readerService;
        }

        // ------------------------------ Get All Checkout -------------------------------
        public async Task<IActionResult> List()
        {
            var readerId = HttpContext.Session.GetInt32("readerId");
            var employeeId = HttpContext.Session.GetInt32("employeeId");
            var role = HttpContext.Session.GetInt32("roleId");

            // Check if user is logged in (either reader or employee)
            if (readerId == null && employeeId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            try
            {
                List<ReservationResponseDTO> reservations;
                
                // If admin or librarian, show all reservations
                if (employeeId != null && (role == 1 || role == 2))
                {
                    reservations = await reservationService.GetAllReservations();
                }
                // If reader, show only their reservations
                else if (readerId != null)
                {
                    reservations = await reservationService.GetReservationsByReader(readerId.Value);
                }
                else
                {
                    
                    return RedirectToAction("Index", "Home");
                }

                return View(reservations);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(new List<ReservationResponseDTO>());
            }
        }

        // -------------------- View Detail -----------------------------------------------
        public async Task<IActionResult> Details(int? id)
        {
            var readerId = HttpContext.Session.GetInt32("readerId");
            var employeeId = HttpContext.Session.GetInt32("employeeId");
            var role = HttpContext.Session.GetInt32("roleId");

            // Check if user is logged in
            if (readerId == null && employeeId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            try
            {
                ReservationResponseDTO reservation;
                
                // If admin or librarian, can view any reservation
                if (employeeId != null && (role == 1 || role == 2))
                {
                    // For admin/librarian, we don't need to check readerId
                    reservation = await reservationService.GetReservationById(null, id);
                }
                // If reader, can only view their own reservations
                else if (readerId != null)
                {
                    reservation = await reservationService.GetReservationById(readerId, id);
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }

                return View(reservation);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View();
            }
        }

        // ----------------------- Create -----------------------------------------------
        [HttpGet]
        public async Task<IActionResult> Create(int? bookId)
        {
            var readerId = HttpContext.Session.GetInt32("readerId");
            var employeeId = HttpContext.Session.GetInt32("employeeId");
            var role = HttpContext.Session.GetString("role");

            // Check if user is logged in
            if (readerId == null && employeeId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var book = await bookService.GetBook(bookId);
            ViewBag.Book = book;

            // If reader, get their info
            if (readerId != null)
            {
                var reader = await readerService.getReaderByID(readerId.Value);
                ViewBag.Reader = reader;
            }
            // If admin/librarian, they can create reservation for any reader
            else if (employeeId != null && (role == "Admin" || role == "Librarian"))
            {
                // Admin can select reader, so we don't set a specific reader
                ViewBag.Reader = null;
                ViewBag.IsAdmin = true;
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ReservationRequestDTO reservationRequestDTO)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Error = "Dữ liệu không hợp lệ";
                return View();
            }

            try
            {
                var book = await bookService.GetBook(reservationRequestDTO.BookId);
                var reader = await readerService.getReaderByID(reservationRequestDTO.ReaderId);

                if (reservationRequestDTO.Quantity > book.Quantity)
                {
                    throw new Exception("Số lượng sách mượn không được cao hơn sách hiện có");
                }

                await reservationService.CreateReservation(reservationRequestDTO);

                ViewBag.Success = "Tạo phiếu đặt sách thành công!";
                ViewBag.Book = book;
                ViewBag.Reader = reader;

                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View();
            }
        }

        //----------------------------- Update Status ----------------------------------

        public async Task<IActionResult> ChangeStatus(int id, string actionType)
        {
            bool ok = await reservationService.UpdateStatus(id, actionType);

            TempData["msg"] = ok
                ? $"Reservation {actionType} successfully!"
                : "Action failed!";

            return RedirectToAction("List");
        }
    }
}
