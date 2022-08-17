using System.Collections.Generic;

namespace FivePointes.Data.Models
{
    public class Portfolio
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<PortfolioPicture> Pictures { get; set; }
    }
}
