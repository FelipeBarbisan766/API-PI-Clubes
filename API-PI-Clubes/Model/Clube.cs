using API_PI_Clubes.Model.ValueObjects;

namespace API_PI_Clubes.Model
{
    public class Clube : BaseEntity
    {
        //public Clube(string name, AddressVO address, int phoneNumber, string description) { Name = name; Address = address; PhoneNumber = phoneNumber; Description = description; }
        public string Name { get; set; }
        public AddressVO Address { get; set; }
        public int PhoneNumber { get; set; }
        public string Description { get; set; }
    }
}
