using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Library.Models;

namespace E_Library.Repository
{
    public interface IReaderRepository
    {
        Task<List<Reader>> GetAll(string? firstName, string? lastName,
                              string? email, string? phoneNumber,
                              bool? status);
        Task<Reader> CreateReader(Reader reader);
        
        Task<Reader> getReaderByEmail(string email);
        Task<Reader> getReaderByPhoneNumber(string phoneNumber);
        Task<Reader> getReaderById(int? readerId);
        Task UpdateReader(Reader reader);
        Task Save();
        Task Delete(int readerID);
    }
}
