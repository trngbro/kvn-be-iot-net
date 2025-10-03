namespace Common.Settings
{
    public class AppSettings
    {
        public required string DomainUrl { get; set; }
        public required string SendEmailUrl { get; set; }
        public required string SmsToken { get; set; }
        public required string SmsServiceUrl { get; set; }
        public required object ClientId { get; set; }
    }

    public class StrJWT
    {
        public required string Issuer { get; set; }
        public required string Audience { get; set; }
        public required string Key { get; set; }
    }

    public class ConnectionStrings
    {
        public required string WebApiDatabase { get; set; }
    }

    public class ApplicationInsights
    {
        public required string ConnectionString { get; set; }
    }
}