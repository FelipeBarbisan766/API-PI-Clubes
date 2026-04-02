namespace API_PI_Clubes.Model.ValueObjects
{
    public class EmailVerificationVO
    {
        public bool IsConfirmed { get; private set; }
        public DateTime? VerifiedAt { get; private set; }

        private EmailVerificationVO(bool isConfirmed, DateTime? verifiedAt)
        {
            IsConfirmed = isConfirmed;
            VerifiedAt = verifiedAt;
        }

        public static EmailVerificationVO Create()
            => new EmailVerificationVO(false, null);

        public static EmailVerificationVO Confirm()
            => new EmailVerificationVO(true, DateTime.UtcNow);
    }
}
