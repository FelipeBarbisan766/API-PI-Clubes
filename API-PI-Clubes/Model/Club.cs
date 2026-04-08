using API_PI_Clubes.Model.ValueObjects;

namespace API_PI_Clubes.Model
{
    public class Club : BaseEntity
    {
        public string Name { get; set; }
        public AddressVO Address { get; set; }
        public string PhoneNumber { get; set; }
        public string Description { get; set; }
        public virtual ICollection<Court> Courts { get; set; }
        public virtual ICollection<ClubAdmin> ClubAdmin { get; set; }
        public virtual ICollection<Image> Images { get; set; }
    }
}
