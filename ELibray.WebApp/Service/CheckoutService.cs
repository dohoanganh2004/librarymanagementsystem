using E_Library.Models;
using E_Library.Repository;
using ELibrary.WebApp.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace ELibrary.WebApp.Service
{
    public class CheckoutService : ICheckoutService
    {
        private readonly ICheckoutRepository _checkoutRepository;
        private readonly IHubContext<NotificationHub> _hub;

        public CheckoutService(ICheckoutRepository checkoutRepository, IHubContext<NotificationHub> hub)
        {
            _checkoutRepository = checkoutRepository;
            _hub = hub;
        }

        // change status
        public async Task<bool> ChangeStatus(int? id, string? actionType)
        {
            var checkout = await _checkoutRepository.GetCheckoutById(id);
            if (checkout == null)
                return false;

            var today = DateOnly.FromDateTime(DateTime.Now);
            string message = "";

            switch (actionType?.ToLower())
            {
                case "earned":
                    checkout.Status = "OnLoan";
                    checkout.BrowseDate = today;
                    message = "Bạn đã nhận sách thành công.";
                    break;

                case "return":
                    checkout.ReturnDate = today;

                    if (checkout.DueDate.HasValue && today > checkout.DueDate.Value)
                    {
                        checkout.Status = "OverdueReturn";
                        message = "Bạn đã trả sách trễ hạn.";
                    }
                    else
                    {
                        checkout.Status = "Returned";
                        message = "Bạn đã trả sách thành công.";
                    }
                    break;

                case "lost":
                    checkout.Status = "Lost";
                    checkout.ReturnDate = null;
                    message = "Bạn đã báo mất sách.";
                    break;

                case "damaged":
                    checkout.Status = "Damaged";
                    checkout.ReturnDate = today;
                    message = "Bạn đã báo sách bị hư hỏng.";
                    break;

                default:
                    return false;
            }

            await _checkoutRepository.UpdateCheckout(checkout);

            // Notify reader
            await _hub.Clients.Group($"reader-{checkout.ReaderId}")
                .SendAsync("ReceiveMessage", "Phiếu mượn", message);

            // Notify librarians and admins
            await _hub.Clients.Group("role-2") 
                .SendAsync("ReceiveMessage", "Quản lý mượn trả", $"Reader #{checkout.ReaderId}: {message}");

            await _hub.Clients.Group("role-1") 
                .SendAsync("ReceiveMessage", "Quản lý mượn trả", $"Reader #{checkout.ReaderId}: {message}");

            return true;
        }


        // get all checkout
        public async Task<List<Checkout>> GetAll()
        {
            return await _checkoutRepository.GetAll();
        }

        // get checkout by id
        public async Task<Checkout> GetCheckoutByID(int? id)
        {
            return await _checkoutRepository.GetCheckoutById(id);
        }

        // get list checkout of reader
        public async Task<List<Checkout>> GetCheckoutByReader(int? readerID)
        {
            if (readerID == null)
                throw new ArgumentNullException(nameof(readerID));

            return await _checkoutRepository.GetCheckoutListByReaderID(readerID);
        }

        // get checkout detail
        public async Task<Checkout> GetCheckoutDetail(int? id)
        {
            return await _checkoutRepository.GetCheckoutById(id);
        }

        public async Task<bool> ChangeStatusBulk(List<int> ids, string actionType)
        {
            bool allSuccess = true;

          

            foreach (var id in ids)
            {
                
                bool success = await ChangeStatus(id, actionType);
                if (!success)
                {
                    allSuccess = false;
                   
                }
            }

            return allSuccess;
        }
    }
}
