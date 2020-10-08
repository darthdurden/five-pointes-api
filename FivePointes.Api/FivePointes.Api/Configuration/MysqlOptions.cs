namespace FivePointes.Api.Configuration
{
    public class MysqlOptions
    {
        public string Server { get; set; }
        public string Database { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string ConnectionString => $"server={Server};database={Database};user={Username};password={Password}";
    }
}
