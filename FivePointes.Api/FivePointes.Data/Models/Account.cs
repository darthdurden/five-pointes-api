using System.Collections.Generic;

namespace FivePointes.Data.Models
{
    public class Account : IDataModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public ICollection<Expense> Expenses { get; set; }
    }
}
