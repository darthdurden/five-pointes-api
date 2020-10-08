namespace FivePointes.Api.Dtos.Hal
{
    public class PictureHalLinks : HalLinks
    {
        public HalLink Full { get; set; }
        public HalLink Thumbnail { get; set; }
    }
}
