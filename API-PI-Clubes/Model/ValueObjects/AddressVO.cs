using System.Diagnostics.Metrics;
using System.Reflection.Emit;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace API_PI_Clubes.Model.ValueObjects
{
    public class AddressVO
    {
        public string ZipCode { get; private set; }
        public string Street { get; private set; }
        public string Number { get; private set; }
        public string Neighborhood { get; private set; }
        public string? Complement { get; private set; }
        public string City { get; private set; }
        public string State { get; private set; }
        public string Country { get; private set; }

        private AddressVO() { }

        public AddressVO(
            string zipCode,
            string street,
            string number,
            string neighborhood,
            string? complement,
            string city,
            string state,
            string country)
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
    }
}