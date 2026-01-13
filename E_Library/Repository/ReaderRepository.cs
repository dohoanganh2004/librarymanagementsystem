using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Library.Models;
using Microsoft.EntityFrameworkCore;

namespace E_Library.Repository
{
    public class ReaderRepository : IReaderRepository
    {
        private readonly ElibraryContext _elibraryContext;

        public ReaderRepository(ElibraryContext elibraryContext)
        {
            _elibraryContext = elibraryContext;
        }

        // create new Reader
        public async Task<Reader> CreateReader(Reader reader)
        {
            await _elibraryContext.AddAsync(reader);
            await _elibraryContext.SaveChangesAsync();
            return reader;
        }

        // get Reader by email
        public async Task<Reader> getReaderByEmail(string email)
        {
            return await _elibraryContext.Readers
                .FirstOrDefaultAsync(r => r.Email.Equals(email));
        }

        public async Task<Reader> getReaderByPhoneNumber(string phoneNumber)
        {
            return await _elibraryContext.Readers
                .FirstOrDefaultAsync(r => r.PhoneNumber == phoneNumber);
        }

        public async Task<Reader> getReaderById(int? readerId)
        {
            if (readerId == null)
                return null;

            return await _elibraryContext.Readers
                .FirstOrDefaultAsync(r => r.ReaderId == readerId.Value);
        }

        public async Task<List<Reader>> GetAll(string? firstName, string? lastName, 
                                               string? email, string? phoneNumber,
                                               bool? status)
        {
            var readers = await _elibraryContext.Readers.ToListAsync();

            if (firstName != null)
            {
                readers = readers
                    .Where(r => r.FirstName != null &&
                                r.FirstName.ToLower().Contains(firstName.ToLower()))
                    .ToList();
            }

            if (lastName != null)
            {
                readers = readers
                    .Where(r => r.LastName != null &&
                                r.LastName.ToLower().Contains(lastName.ToLower()))
                    .ToList();
            }

            

            if (email != null)
            {
                readers = readers
                    .Where(r => r.Email != null &&
                                r.Email.ToLower().Contains(email.ToLower()))
                    .ToList();
            }

            if (phoneNumber != null)
            {
                readers = readers
                    .Where(r => r.PhoneNumber != null &&
                                r.PhoneNumber.ToLower().Contains(phoneNumber.ToLower()))
                    .ToList();
            }

            if (status != null)
            {
                readers = readers
                    .Where(r => r.Status == status.Value)
                    .ToList();
            }

            return readers;
        }

        public async Task Save()
        {
            await _elibraryContext.SaveChangesAsync();
        }

        public async Task Delete(int readerID)
        {
            var deleteReader = await getReaderById(readerID);
            if (deleteReader != null)
            {
                _elibraryContext.Remove(deleteReader);
                await _elibraryContext.SaveChangesAsync();
            }
        }

        public async Task UpdateReader(Reader reader)
        {
            _elibraryContext.Update(reader);
            await _elibraryContext.SaveChangesAsync();
        }
    }
}
