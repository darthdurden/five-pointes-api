using AutoMapper;
using FivePointes.Api.Dtos.Hal;
using Microsoft.AspNetCore.Mvc.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace FivePointes.Api.Mappings.Resolvers
{
    public class HalLinkResolver : IMemberValueResolver<object, object, HalLinkContext, HalLink>
    {
        public HalLink Resolve(object source, object destination, HalLinkContext sourceMember, HalLink destMember, ResolutionContext context)
        {
            if(sourceMember == null)
            {
                return null;
            }

            return new HalLink
            {
                Href = GetHref(sourceMember)
            };
        }

        private Uri GetHref(HalLinkContext halContext)
        {
            var methodInfo = halContext.ControllerType.GetMethod(halContext.MethodName);
            if(methodInfo == null)
            {
                throw new ArgumentException($"Could not find method name '{halContext.MethodName}' for HalLink");
            }
            
            var sb = GetTemplatedPath(methodInfo, halContext.ControllerType);
            sb.Replace("[controller]", $"{halContext.ControllerType.Name.Replace("Controller", "").ToLowerInvariant()}");
            sb.Replace("[action]", $"{halContext.MethodName.ToLowerInvariant()}");
            ReplaceRouteVariables(sb, halContext.PathParameters);
            AddQueryParameters(sb, halContext.QueryParameters);

            return new Uri($"https://api.lookatmycode.com{sb}");
        }

        private StringBuilder GetTemplatedPath(params ICustomAttributeProvider[] providers)
        {
            StringBuilder sb = new StringBuilder();

            var hasRootedPath = false;
            var templates = new List<string>();
            foreach(var provider in providers)
            {
                var attribute = provider.GetCustomAttributes(typeof(IRouteTemplateProvider), true).Cast<IRouteTemplateProvider>().OrderBy(x => x.Order).FirstOrDefault(x => x.Template != null);
            
                if(attribute != null)
                {
                    templates.Add(attribute.Template);

                    // If this is a rooted route, then we don't need to continue looking
                    if(attribute.Template.StartsWith('/'))
                    {
                        hasRootedPath = true;
                        break;
                    }
                }
            }

            var addRoot = !hasRootedPath;

            templates.Reverse();
            foreach(var template in templates)
            {
                if (addRoot)
                {
                    sb.Append('/');
                }
                sb.Append(template);
                addRoot = true;
            }

            return sb;
        }

        private static void ReplaceRouteVariables(StringBuilder sb, Dictionary<string, string> routeParameters)
        {
            if(routeParameters == null || routeParameters.Count == 0)
            {
                return;
            }

            foreach(var parameter in routeParameters)
            {
                sb.Replace($"{{{parameter.Key}}}", parameter.Value);
            }
        }

        private static void AddQueryParameters(StringBuilder sb, Dictionary<string, string> queryParameters)
        {
            if (queryParameters == null)
            {
                return;
            }

            var separator = "?";

            foreach (var parameter in queryParameters)
            {
                sb.Append(separator);
                sb.Append(parameter.Key);
                sb.Append("=");
                sb.Append(parameter.Value);

                separator = "&";
            }
        }
    }
}
