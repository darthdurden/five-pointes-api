namespace FivePointes.Logic.Models
{
    public class TransactionCategory : IModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public TransactionType TransactionType { get; set; }
    }
}
