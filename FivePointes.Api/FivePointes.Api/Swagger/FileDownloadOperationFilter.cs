using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FivePointes.Api.Swagger
{
    public class FileDownloadOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            foreach(var responsePair in operation.Responses)
            {
                var statusKey = responsePair.Key;
                var attr = context.MethodInfo.GetCustomAttributes().OfType<ProducesFileResponseAttribute>().SingleOrDefault(x => x.StatusCode.ToString() == statusKey);
            
                if(attr != null)
                {
                    foreach(var contentType in attr.ContentTypes)
                    {
                        responsePair.Value.Content[contentType] = new OpenApiMediaType
                        {
                            Schema = context.SchemaGenerator.GenerateSchema(attr.Type, context.SchemaRepository)
                        };
                    }

                    var otherContentTypes = responsePair.Value.Content.Keys.Where(x => !attr.ContentTypes.Contains(x)).ToList();
                    foreach(var otherContentType in otherContentTypes)
                    {
                        var toRemove = responsePair.Value.Content[otherContentType];
                        responsePair.Value.Content.Remove(new KeyValuePair<string, OpenApiMediaType>(otherContentType, toRemove));
                    }
                }
            }
        }
    }
}
