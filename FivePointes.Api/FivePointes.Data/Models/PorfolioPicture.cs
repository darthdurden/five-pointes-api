namespace FivePointes.Data.Models
{
    public class PortfolioPicture
    {
        public int Id { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int ThumbWidth { get; set; }
        public int ThumbHeight { get; set; }
        public int SortIndex { get; set; }
        public int PortfolioId { get; set; }
        public Portfolio Portfolio { get; set; }
    }
}
