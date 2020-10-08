namespace FivePointes.Logic.Models
{
    public interface IUser
    {
        public long Id { get; set; }
        bool IsAdmin { get; }
    }
}
