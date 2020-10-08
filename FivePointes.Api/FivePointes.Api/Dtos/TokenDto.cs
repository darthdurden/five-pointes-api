namespace FivePointes.Api.Dtos
{
    public class TokenDto
    {
        public string Username { internal get; set; }
        public string Password { internal get; set; }
        public string AccessToken { get; internal set; }
    }
}
