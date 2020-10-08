namespace FivePointes.Logic.Models
{
    public class Account : IModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public decimal Total { get; set; }
        public decimal ClearedTotal { get; set; }
    }
}
