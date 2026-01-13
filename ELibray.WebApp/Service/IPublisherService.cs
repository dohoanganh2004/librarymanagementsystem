using E_Library.Models;

namespace ELibrary.WebApp.Service
{
    public interface IPublisherService
    {
     Task<List<Publisher>> GetAll();
    }
}
