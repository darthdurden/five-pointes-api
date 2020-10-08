using System;

namespace FivePointes.Data.Models
{
    public class Expense : IDataModel
    {
        public long Id { get; set; }
        public string Description { get; set; }
        public DateTime DatePaid { get; set; }
        public decimal Price { get; set; }
        public bool IsCleared { get; set; }

        public ExpenseCategory Category { get; set; }
        public long? CategoryId { get; set; }

        public Account Account { get; set; }
        public long? AccountId { get; set; }
    }
}
