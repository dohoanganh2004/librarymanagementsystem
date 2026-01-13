using System.Runtime.CompilerServices;
using ELibrary.WebApp.Service;
using Microsoft.AspNetCore.Mvc;

namespace ELibrary.WebApp.Controllers
{
    public class ReaderController : Controller
    {
        private readonly IReaderService _readerService;

        public ReaderController(IReaderService readerService)
        {
            _readerService = readerService;
        }

        //--------------------------- Get All Reader --------------------------------------
        public async Task<IActionResult> All(string? firstName, string? lastName,
                                             string? email, string? phoneNumber,
                                             bool? status)
        {
            var readers = await _readerService.getAll(firstName, lastName,  email, phoneNumber, status);
            return View(readers);
        }

        //---------------------------- Detail Reader ---------------------------------------
        public async Task<IActionResult> Details(int? id)
        {
            var reader = await _readerService.getReaderByID(id);
            return View(reader);
        }


        //-------------- Change Status of Reader ---------------------
        public async Task<IActionResult> ChangeStatus(int?id, string?actionType)
        {
            bool ok = await _readerService.changeStatus(id,actionType);

            TempData["msg"] = ok
                ? $"Action successfully!"
                : "Action failed!";

            return RedirectToAction("All");
        }
    }
}
