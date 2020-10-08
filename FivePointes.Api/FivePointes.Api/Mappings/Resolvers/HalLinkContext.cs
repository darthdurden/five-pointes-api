using System;
using System.Collections.Generic;

namespace FivePointes.Api.Mappings.Resolvers
{
    public class HalLinkContext
    {
        public Type ControllerType { get; set; }
        public string MethodName { get; set; }
        public Dictionary<string, string> PathParameters { get; set; }
        public Dictionary<string, string> QueryParameters { get; set; }
    }
}
