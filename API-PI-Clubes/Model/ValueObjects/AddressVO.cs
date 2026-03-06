using System.Diagnostics.Metrics;
using System.Reflection.Emit;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace API_PI_Clubes.Model.ValueObjects
{
    public class AddressVO
    {
        public AddressVO() { }
        public AddressVO(string zipCode, string street, string number, string neighborhood, string complement, string city, string state, string country)
        {
            ZipCode = zipCode;
            Street = street;
            Number = number;
            Neighborhood = neighborhood;
            Complement = complement;
            City = city;
            State = state;
            Country = country;

        }
        public string? ZipCode { get; set; }
        public string? Street { get; set; }
        public string? Number { get; set; }
        public string? Neighborhood { get; set; }
        public string? Complement { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
    }
}
