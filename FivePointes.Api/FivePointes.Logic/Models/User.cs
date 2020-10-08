using System.Collections.Generic;

namespace FivePointes.Logic.Models
{
    public class User : IUser
    {
        public long Id { get; set; }
        public string Username { get; set; }
        public string Salt { get; set; }
        public string Hash { get; set; }
        public bool IsAdmin { get; set; }
    }
}
