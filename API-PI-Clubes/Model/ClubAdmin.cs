namespace API_PI_Clubes.Model
{
    public class ClubAdmin
    {
        public Guid AdminId { get; set; }
        public Admin Admin { get; set; }

        public Guid ClubId { get; set; }
        public Club Club { get; set; }
    }
}
