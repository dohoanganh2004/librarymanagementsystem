using ELibrary.WebApp.Service;
using Microsoft.AspNetCore.Mvc;

namespace ELibrary.WebApp.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly ICheckoutService _checkoutService;

        public CheckoutController(ICheckoutService checkoutService)
        {
            _checkoutService = checkoutService;
        }

        //------------------ List of Checkout -------------------------
        public async Task<IActionResult> All()
        {
            var checkouts = await _checkoutService.GetAll();
            return View(checkouts);
        }

        //------------------ My Checkouts (for readers) -------------------------
        public async Task<IActionResult> MyCheckouts()
        {
            var readerId = HttpContext.Session.GetInt32("readerId");
            if (readerId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            try
            {
                var checkouts = await _checkoutService.GetCheckoutByReader(readerId);
                return View(checkouts);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(new List<E_Library.Models.Checkout>());
            }
        }

        // --------------- View Detail ---------------------------------
        public async Task<IActionResult> Details(int? id)
        {
            var checkout = await _checkoutService.GetCheckoutByID(id);
            return View(checkout);
        }

        // ------------------- Change Status ---------------------------
        [HttpPost]
        public async Task<IActionResult> ChangeStatus(int? id, string? actionType)
        {
            if (id == null || string.IsNullOrEmpty(actionType))
            {
                TempData["Error"] = "Invalid request!";
                return RedirectToAction("All");
            }

            bool isSuccess = await _checkoutService.ChangeStatus(id.Value, actionType);

            if (isSuccess)
            {
                var actionText = actionType.ToLower() switch
                {
                    "earned" => "duyệt và chuyển sang Đang mượn",
                    "return" => "trả sách",
                    "lost" => "đánh dấu mất",
                    "damaged" => "đánh dấu hỏng",
                    _ => actionType
                };
                TempData["Success"] = $"Đã {actionText} phiếu mượn #{id} thành công.";
            }
            else
            {
                TempData["Error"] = "Không thể cập nhật trạng thái phiếu mượn.";
            }

            return RedirectToAction("All");
        }
    
    // --------------- Chan Status hàng loat ----------------------

    [HttpPost]
        public async Task<IActionResult> ChangeStatusBulk(string ids, string actionType)
        {
            if (string.IsNullOrEmpty(ids) || string.IsNullOrEmpty(actionType))
            {
                return BadRequest("Missing IDs or action type.");
            }

            var checkoutIds = ids.Split(',')
                                 .Select(idStr => int.TryParse(idStr, out int id) ? (int?)id : null)
                                 .Where(id => id.HasValue)
                                 .Select(id => id.Value)
                                 .ToList();

            if (!checkoutIds.Any())
            {
                return BadRequest("No valid IDs provided.");
            }

            bool allSuccess = await _checkoutService.ChangeStatusBulk(checkoutIds, actionType);

            if (allSuccess)
            {
                TempData["Success"] = $"Đã cập nhật trạng thái của {checkoutIds.Count} phiếu mượn thành công.";
                return Ok();
            }
            else
            {
                TempData["Error"] = "Thất bại khi cập nhật trạng thái hàng loạt. Một số phiếu có thể đã được cập nhật.";
                return StatusCode(500, "Failed to update all statuses.");
            }
        }
    }
}
