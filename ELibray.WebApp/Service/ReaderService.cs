using AutoMapper;
using E_Library.Models;
using E_Library.Repository;
using ELibrary.WebApp.DTO.Request;
using ELibrary.WebApp.DTO.Response;
using ELibrary.WebApp.Models;

namespace ELibrary.WebApp.Service
{
    public class ReaderService : IReaderService
    {
        private readonly IReaderRepository _readerRepository;
        private readonly IMapper _mapper;

        public ReaderService(IReaderRepository readerRepository, IMapper mapper)
        {
            _readerRepository = readerRepository;
            _mapper = mapper;
        }
        // change status of reader 
        

        public async Task<bool> changeStatus(int? readerId, string activityType)
        {
            if (readerId == null || string.IsNullOrWhiteSpace(activityType))
                return false;

            var reader = await _readerRepository.getReaderById(readerId);
            if (reader == null)
                return false;

            switch (activityType.ToLower())
            {
                case "lock":
                    reader.Status = false;
                    break;

                case "unlock":
                    reader.Status = true;
                    break;

                default:
                    return false;
            }

            await _readerRepository.UpdateReader(reader);
            return true;
        }



        // get all reader 
        public async Task<List<ReaderResponseDTO>> getAll(string? firstName, string? lastName, 
                                              string? email, string? phoneNumber, bool? status)
        {
            var readers =  await _readerRepository.GetAll(firstName,lastName, email, phoneNumber,status);
            
            return _mapper.Map<List<ReaderResponseDTO>>(readers);
        }

        // lấy thông tin Reader theo ID
        public async Task<ReaderResponseDTO> getReaderByID(int? readerId)
        {
            var reader = await _readerRepository.getReaderById(readerId);

            if (reader == null)
                throw new Exception("Không tìm thấy người dùng với id = " + readerId);

            return _mapper.Map<ReaderResponseDTO>(reader);
        }

        // update Reader
        public async Task UpdateReader(ReaderUpdateRequestDTO readerUpdateRequestDTO)
        {
            var existReader = await _readerRepository.getReaderById(readerUpdateRequestDTO.ReaderId);

            if (existReader == null)
                throw new Exception("Không tìm thấy reader với id " + readerUpdateRequestDTO.ReaderId);

            var phoneExist = await _readerRepository.getReaderByPhoneNumber(readerUpdateRequestDTO.PhoneNumber);

            if (phoneExist != null && phoneExist.ReaderId != readerUpdateRequestDTO.ReaderId)
            {
                throw new Exception("Số điện thoại này đã tồn tại, vui lòng chọn số khác!");
            }

            _mapper.Map(readerUpdateRequestDTO, existReader);

            await _readerRepository.UpdateReader(existReader);
        }


      

      
    }
}
  