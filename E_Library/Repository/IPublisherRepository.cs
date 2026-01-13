using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Library.Models;

namespace E_Library.Repository
{
    public interface IPublisherRepository
    {
        Task<List<Publisher>> GetAll();
    }
}
