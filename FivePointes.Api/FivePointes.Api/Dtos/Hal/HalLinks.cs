using FivePointes.Api.Swagger;
using System.ComponentModel;

namespace FivePointes.Api.Dtos.Hal
{
    [ReadOnly(true)]
    [InlineSchema]
    public class HalLinks
    {
        public HalLink Self { get; set; }
    }
}
