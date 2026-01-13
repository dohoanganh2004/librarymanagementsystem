using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Library.Models;
using Microsoft.EntityFrameworkCore;

namespace E_Library.Repository
{
    public class CheckoutRepository : ICheckoutRepository
    {
        private readonly ElibraryContext _context;

        public CheckoutRepository(ElibraryContext context)
        {
            _context = context;
        }

        //----------- delete checkout 
        public async Task DeleteCheckout(int? checkoutId)
        {
            var deleteCheckout = await GetCheckoutById(checkoutId);
            if (deleteCheckout != null)
            {
                _context.Remove(deleteCheckout);
                await _context.SaveChangesAsync();
            }
        }

        //----------------- get checkout by id
        public async Task<Checkout?> GetCheckoutById(int? id)
        {
            return await _context.Checkouts
                .FirstOrDefaultAsync(c => c.CheckoutId == id);
        }

        // --------- Get Checkout by reader id
        public async Task<List<Checkout>> GetCheckoutListByReaderID(int? readerID)
        {
            return await _context.Checkouts
                .Where(c => c.ReaderId == readerID)
                .Include(c => c.Reader)
                .Include(c => c.Book)
                .ToListAsync();
        }

        // ----- Create new checkout 
        public async Task<Checkout> NewCheckout(Checkout checkout)
        {
            await _context.AddAsync(checkout);
            return checkout;
        }

        //-------- Get All Checkout
        public async Task<List<Checkout>> GetAll()
        {
            return await _context.Checkouts
                .Include(c => c.Reader)
                .Include(c => c.Book)
                .ToListAsync();
        }

        // ---------------- Update checkout ---------------------
        public async Task UpdateCheckout(Checkout checkout)
        {
            _context.Update(checkout);  // EF Core không có UpdateAsync()
            await _context.SaveChangesAsync();
        }
    }
}
