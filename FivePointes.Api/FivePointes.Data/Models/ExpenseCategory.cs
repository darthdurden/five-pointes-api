using System.Collections.Generic;

namespace FivePointes.Data.Models
{
    public class ExpenseCategory : IDataModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public int Type { get; set; }
        public ICollection<Expense> Expenses { get; set; }
    }
}
