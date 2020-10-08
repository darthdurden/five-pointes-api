namespace FivePointes.Api.Dtos.Hal
{
    public class AccountHalLinks : HalLinks
    {
        public HalLink RecentTransactions { get; set; }
        public HalLink UnclearedTransactions { get; set; }
    }
}
