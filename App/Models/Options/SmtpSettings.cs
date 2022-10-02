namespace MyWishly.App.Models.Options
{
    public class SmtpSettings
    {
        public string? SmtpHost { get; set; }
        public string? Username { get; set; }
        public string? ApiKey { get; set; }
        public int SmtpPort { get; set; }
        public bool UseTls { get; set; }
        public string? FromAddress { get; set; }
    }
}
