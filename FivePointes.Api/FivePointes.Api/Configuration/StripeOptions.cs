using System.Collections.Generic;

namespace FivePointes.Api.Configuration
{
    public class StripeOptions
    {
        public Dictionary<string, string> ApiKeys { get; set; }
        public Dictionary<string, string> WebhookSecrets { get; set; }
    }
}
