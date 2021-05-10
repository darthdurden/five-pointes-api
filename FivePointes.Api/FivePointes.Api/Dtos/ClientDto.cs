namespace FivePointes.Api.Dtos
{
    public class ClientDto
    {
        public string Id { get; internal set; }
        public string Name { get; internal set; }
        public double CommittedHours { get; set; }
        public bool IsHidden { get; set; }
    }
}
