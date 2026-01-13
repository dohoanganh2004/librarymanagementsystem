using E_Library.Models;
using E_Library.Repository;

namespace ELibrary.WebApp.Service
{
    public class PublisherService : IPublisherService
    {

        private readonly IPublisherRepository _publisherRepository;

        public PublisherService(IPublisherRepository publisherRepository)
        {
            _publisherRepository = publisherRepository; 
        }
        public Task<List<Publisher>> GetAll()
        {
            return  _publisherRepository.GetAll();
        }
    }
}
