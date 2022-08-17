namespace FivePointes.Logic.Configuration
{
    public class PortfolioOptions
    {
        public string StorageRoot { get; set; }
        public string ThumbStorageTemplate { get; set; }
        public string FullStorageTemplate { get; set; }
        public int? ThumbnailMaxWidth { get; set; }
        public int? ThumbnailMaxHeight { get; set; }
    }
}
