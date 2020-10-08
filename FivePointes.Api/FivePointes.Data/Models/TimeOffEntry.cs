using System;

namespace FivePointes.Data.Models
{
    public class TimeOffEntry : IDataModel
    {
        public long Id { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string Description { get; set; }
    }
}
