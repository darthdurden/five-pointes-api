using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace FivePointes.Api.Swagger
{
    public class ProducesFileResponseAttribute : ProducesResponseTypeAttribute
    {
        public string[] ContentTypes { get; set; }

        public ProducesFileResponseAttribute(int statusCode) : this(statusCode, "application/octet-stream") { }

        public ProducesFileResponseAttribute(int statusCode, params string[] contentTypes) : base(typeof(Stream), statusCode)
        {
            ContentTypes = contentTypes;
        }
    }
}
