namespace FivePointes.Api.Dtos.Hal
{
    public class TransactionHalLinks : HalLinks
    {
        public HalLink Account { get; set; }
        public HalLink Category { get; set; }
    }
}
