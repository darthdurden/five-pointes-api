namespace FivePointes.Logic.Models
{
    public class PortfolioPicture
    {
        public int Id { get; set; }
        public int PortfolioId { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int ThumbnailWidth { get; set; }
        public int ThumbnailHeight { get; set; }
        public int SortIndex { get; set; }
    }
}
