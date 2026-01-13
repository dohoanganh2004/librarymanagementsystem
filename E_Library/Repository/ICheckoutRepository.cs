using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Library.Models;

namespace E_Library.Repository
{
    public interface ICheckoutRepository
    {
        Task<Checkout> NewCheckout(Checkout checkout);
       Task<List<Checkout>> GetCheckoutListByReaderID(int?readerID);
       
       Task<Checkout> GetCheckoutById(int ?id);

        Task DeleteCheckout(int ? checkoutId);

         Task<List<Checkout>> GetAll();

        Task UpdateCheckout(Checkout checkout);



    }
}
