using E_Library.Models;

namespace ELibrary.WebApp.Service
{
    public interface ICheckoutService
    {
        Task<List<Checkout>> GetCheckoutByReader(int? readerID);
        Task<Checkout> GetCheckoutDetail( int? id);

        Task<Checkout> GetCheckoutByID(int ?id);
        Task<List<Checkout>> GetAll();

        Task<bool> ChangeStatus(int? id , string ? actionType);

         Task<bool> ChangeStatusBulk(List<int> ids, string actionType);


    }
}
