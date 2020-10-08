using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json.Serialization;

namespace FivePointes.Api.Swagger
{
    public class InlineSchemaFilter : ISchemaFilter, IDocumentFilter
    {
        private List<string> _schemasToRemove;
        public InlineSchemaFilter(List<string> schemasToRemove)
        {
            _schemasToRemove = schemasToRemove;
        }

        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if(schema.Properties != null)
            {
                var typeProps = context.Type.GetProperties(BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                var propInfoLookup = new Dictionary<string, PropertyInfo>(StringComparer.OrdinalIgnoreCase);
                foreach(var typeProp in typeProps)
                {
                    var propNameAttr = typeProp.GetCustomAttribute<JsonPropertyNameAttribute>();
                    if(propNameAttr != null)
                    {
                        propInfoLookup[propNameAttr.Name] = typeProp;
                    }
                    else
                    {
                        propInfoLookup[typeProp.Name] = typeProp;
                    }
                }

                foreach (var kvp in schema.Properties.ToList())
                {
                    var propName = kvp.Key;
                    var propSchema = kvp.Value;

                    if(propInfoLookup.TryGetValue(propName, out var pi))
                    {
                        var inlineSchemaAttr = pi.PropertyType.GetCustomAttribute<InlineSchemaAttribute>(true);
                        if(inlineSchemaAttr != null)
                        {
                            if(context.SchemaRepository.TryGetIdFor(pi.PropertyType, out var schemaId) && context.SchemaRepository.Schemas.TryGetValue(schemaId, out var inlinedSchema))
                            {
                                schema.Properties[propName] = inlinedSchema;

                                if (!_schemasToRemove.Contains(schemaId))
                                {
                                    _schemasToRemove.Add(schemaId);
                                }
                            }
                        }
                    }
                }
            }
        }

        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            foreach(var schemaId in _schemasToRemove)
            {
                swaggerDoc.Components.Schemas.Remove(schemaId);
            }
        }
    }
}
