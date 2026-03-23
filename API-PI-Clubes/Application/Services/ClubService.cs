using API_PI_Clubes.Application.DTOs;
using API_PI_Clubes.Application.Interfaces;
using API_PI_Clubes.Infrastructure.Data;
using API_PI_Clubes.Model;
using API_PI_Clubes.Model.ValueObjects;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace API_PI_Clubes.Application.Services
{
    public class ClubService : IClubService
    {
        private readonly AppDbContext _context;
        public ClubService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ResponseClubDTO>> GetAll()
        {
            return await _context.Clubs
                .Where(c => c.IsActive)
                .Select(c => new ResponseClubDTO
                {
                    Id = c.Id,
                    Name = c.Name,
                    PhoneNumber = c.PhoneNumber,
                    ZipCode = c.Address.ZipCode,
                    Street = c.Address.Street,
                    Number = c.Address.Number,
                    Neighborhood = c.Address.Neighborhood,
                    Complement = c.Address.Complement,
                    City = c.Address.City,
                    State = c.Address.State,
                    Country = c.Address.Country,
                    Description = c.Description
                })
                .ToListAsync();
        }
        public async Task<ResponseClubDTO> GetById(Guid id)
        {
            var data = await _context.Clubs
                .Where(c => c.Id == id && c.IsActive)
                .Select(c => new ResponseClubDTO
                {
                    Id = c.Id,
                    Name = c.Name,
                    PhoneNumber = c.PhoneNumber,
                    ZipCode = c.Address.ZipCode,
                    Street = c.Address.Street,
                    Number = c.Address.Number,
                    Neighborhood = c.Address.Neighborhood,
                    Complement = c.Address.Complement,
                    City = c.Address.City,
                    State = c.Address.State,
                    Country = c.Address.Country,
                    Description = c.Description
                })
                .FirstOrDefaultAsync();

            if (data == null)
                throw new Exception("Club not found");

            return data;
        }

        public async Task Create(CreateClubDTO dto)
        {
            var entity = new Club
            {
                Name = dto.Name,
                PhoneNumber = dto.PhoneNumber,
                Address = new AddressVO(
                   dto.ZipCode,
                   dto.Street,
                   dto.Number,
                   dto.Neighborhood,
                   dto.Complement,
                   dto.City,
                   dto.State,
                   dto.Country
                ),
                Description = dto.Description
            };
            
            _context.Clubs.Add(entity);
            await _context.SaveChangesAsync();
            
            var clubAdmin = new ClubAdmin
            {
                ClubId = entity.Id,
                AdminId = dto.adminId
            };

            _context.ClubAdmins.Add(clubAdmin);
            await _context.SaveChangesAsync();

        }

        public async Task<ResponseClubDTO> Update(Guid id, UpdateClubDTO dto)
        {
            var data = await _context.Clubs.FirstOrDefaultAsync(c => c.Id == id);

            if (data == null)
                throw new Exception("Club not found");

            data.Name = dto.Name;
            data.PhoneNumber = dto.PhoneNumber;
            data.Address = new AddressVO(
                   dto.ZipCode,
                   dto.Street,
                   dto.Number,
                   dto.Neighborhood,
                   dto.Complement,
                   dto.City,
                   dto.State,
                   dto.Country
               );
            data.Description = dto.Description;
            data.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new ResponseClubDTO
            {
                Id =            data.Id,
                Name =          data.Name,
                PhoneNumber =   data.PhoneNumber,
                ZipCode =       data.Address.ZipCode,
                Street =        data.Address.Street,
                Number =        data.Address.Number,
                Neighborhood =  data.Address.Neighborhood,
                Complement =    data.Address.Complement,
                City =          data.Address.City,
                State =         data.Address.State,
                Country =       data.Address.Country,
                Description =   data.Description
            };
        }

        public async Task Delete(Guid id)
        {
            var data = await _context.Clubs.FirstOrDefaultAsync(c => c.Id == id);

            if (data == null)
                throw new Exception("Club not found");

            data.IsActive = false;
            data.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }


    }
}
