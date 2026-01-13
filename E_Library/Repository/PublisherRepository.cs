using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Library.Models;
using Microsoft.EntityFrameworkCore;

namespace E_Library.Repository
{
    public class PublisherRepository :IPublisherRepository
    {
        private readonly ElibraryContext _context;
        public PublisherRepository(ElibraryContext context)
        {
            _context = context;
        }

        public Task<List<Publisher>> GetAll()
        {
           return  _context.Publishers.ToListAsync();

        }
    }
}
