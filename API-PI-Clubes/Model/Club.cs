using API_PI_Clubes.Model.ValueObjects;

namespace API_PI_Clubes.Model
{
    public class Club : BaseEntity
    {
        public string Name { get; set; }
        public AddressVO Address { get; set; }
        public string PhoneNumber { get; set; }
        public string Description { get; set; }

        public ICollection<Court> Courts { get; set; }
        public ICollection<ClubAdmin> ClubAdmin { get; set; }
    }
}
