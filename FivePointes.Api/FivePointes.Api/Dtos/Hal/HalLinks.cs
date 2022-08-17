using FivePointes.Api.Swagger;
using System.ComponentModel;

namespace FivePointes.Api.Dtos.Hal
{
    [ReadOnly(true)]
    public class HalLinks
    {
        public HalLink Self { get; set; }
    }
}
