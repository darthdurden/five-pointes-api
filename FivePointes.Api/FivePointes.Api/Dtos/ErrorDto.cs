using System.Collections.Generic;

namespace FivePointes.Api.Dtos
{
    public class ErrorDto
    {
        public IEnumerable<string> Errors { get; set; }
    }
}
