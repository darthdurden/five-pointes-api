namespace FivePointes.Api.Configuration
{
    public class JwtOptions
    {
        public string Audience { get; set; }
        public string Issuer { get; set; }
        public string Secret { get; set; }
        public int DurationInHours { get; set; }
    }
}
