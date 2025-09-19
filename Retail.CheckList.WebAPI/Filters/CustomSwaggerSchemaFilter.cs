using System;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Runtime.Serialization;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Retail.CheckList.WebAPI.Filters
{
	public class CustomSwaggerSchemaFilter: ISchemaFilter
    {
		public CustomSwaggerSchemaFilter()
		{
		}

        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (schema?.Properties == null)
            {
                return;
            }

            var ignoreDataMemberProperties = context.Type.GetProperties()
                .Where(t => t.GetCustomAttribute<IgnoreDataMemberAttribute>() != null);

            foreach (var ignoreDataMemberProperty in ignoreDataMemberProperties)
            {
                var propertyToHide = schema.Properties.Keys
                    .SingleOrDefault(x => x.ToLower() == ignoreDataMemberProperty.Name.ToLower());

                if (propertyToHide != null)
                {
                    schema.Properties.Remove(propertyToHide);
                }
            }
        }
    }
}

