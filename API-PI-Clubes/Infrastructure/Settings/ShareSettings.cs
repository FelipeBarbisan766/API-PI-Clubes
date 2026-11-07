namespace API_PI_Clubes.Infrastructure.Settings
{
    public class ShareSettings
    {
        public const string SectionName = "Share";

        public string FrontendBaseUrl { get; set; } = string.Empty;

        public string ApiPublicBaseUrl { get; set; } = string.Empty;

        public List<string> AllowedImageHosts { get; set; } = new();
    }
}