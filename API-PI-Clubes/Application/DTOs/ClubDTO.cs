using API_PI_Clubes.Model.Enums;
using Microsoft.Identity.Client;

namespace API_PI_Clubes.Application.DTOs
{
   
    public class CreateClubDTO
    {
        public Guid adminId { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Description { get; set; }
        public string ZipCode { get; set; }
        public string Street { get; set; }
        public string Number { get; set; }
        public string Neighborhood { get; set; }
        public string? Complement { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }

        public List<IFormFile> Images { get; set; }
    }

    public class UpdateClubDTO
    {
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Description { get; set; }
        public string ZipCode { get; set; }
        public string Street { get; set; }
        public string Number { get; set; }
        public string Neighborhood { get; set; }
        public string? Complement { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
    }
    public class ResponseClubDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Description { get; set; }
        public string ZipCode { get; set; }
        public string Street { get; set; }
        public string Number { get; set; }
        public string Neighborhood { get; set; }
        public string? Complement { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        
        public decimal MinPrice { get; set; }
        public int CourtCount { get; set; }
        public List<TypeEnum> Types { get; set; }
    }
    public class ResponseClubByIdDTO
    {
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Description { get; set; }
        public string ZipCode { get; set; }
        public string Street { get; set; }
        public string Number { get; set; }
        public string Neighborhood { get; set; }
        public string? Complement { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public List<ResponseCourtDTO> Courts { get; set; } = new();
    }
}
