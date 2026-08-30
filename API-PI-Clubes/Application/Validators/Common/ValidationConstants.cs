namespace API_PI_Clubes.Application.Validators.Common
{
    public static class ValidationConstants
    {
        public const int NameMinLength = 3;
        public const int NameMaxLength = 150;
        public const int DescriptionMaxLength = 2000;
        public const int PhoneNumberMaxLength = 20;
        public const int AddressFieldMaxLength = 150;
        public const int ComplementMaxLength = 100;


        public const decimal MinPrice = 0.01m;
        public const decimal MaxPricePerHour = 10_000m;
        public const decimal MaxPlanPrice = 100_000m;


        public const int MinPageSize = 1;
        public const int MaxPageSize = 50;


        public const int MaxImagesPerUpload = 10;
        public const long MaxImageSizeBytes = 5 * 1024 * 1024; // 5MB

        public static readonly string[] AllowedImageContentTypes =
        {
            "image/jpeg", "image/png", "image/webp"
        };


        public const int MinSlotDurationMinutes = 15;
        public const int MaxSlotDurationMinutes = 240;


        public static readonly string[] ValidUFs =
        {
            "AC", "AL", "AP", "AM", "BA", "CE", "DF", "ES", "GO",
            "MA", "MT", "MS", "MG", "PA", "PB", "PR", "PE", "PI",
            "RJ", "RN", "RS", "RO", "RR", "SC", "SP", "SE", "TO"
        };
    }
}