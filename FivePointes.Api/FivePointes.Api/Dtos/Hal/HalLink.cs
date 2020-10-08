using FivePointes.Api.Swagger;
using System;

namespace FivePointes.Api.Dtos.Hal
{
    [InlineSchema]
    public class HalLink
    {
        public Uri Href { get; set; }
    }
}
