using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text.Json.Serialization;

namespace FivePointes.Api.Swagger
{
    public class ReadOnlySchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if(context.Type.GetCustomAttributes(true).OfType<ReadOnlyAttribute>().Where(x => x.IsReadOnly).Any())
            {
                schema.ReadOnly = true;
            }
            else
            {
                var typeProps = context.Type.GetProperties(BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                var readOnlyPropNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                foreach (var typeProp in typeProps)
                {
                    if(typeProp.GetCustomAttributes(true).OfType<ReadOnlyAttribute>().Where(x => x.IsReadOnly).Any())
                    {
                        var propNameAttr = typeProp.GetCustomAttribute<JsonPropertyNameAttribute>();
                        if (propNameAttr != null)
                        {
                            readOnlyPropNames.Add(propNameAttr.Name);
                        }
                        else
                        {
                            readOnlyPropNames.Add(typeProp.Name);
                        }
                    }
                }

                foreach (var kvp in schema.Properties.ToList())
                {
                    var propName = kvp.Key;
                    var propSchema = kvp.Value;

                    if(readOnlyPropNames.Contains(propName))
                    {
                        propSchema.ReadOnly = true;
                    }
                }
            }
        }
    }
}
