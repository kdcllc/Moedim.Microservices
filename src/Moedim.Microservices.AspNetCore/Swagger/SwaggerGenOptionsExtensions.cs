using Microsoft.OpenApi.Models;

using Swashbuckle.AspNetCore.SwaggerGen;

namespace Microsoft.Extensions.DependencyInjection;

public static class SwaggerGenOptionsExtensions
{
    public static SwaggerGenOptions AddBearer(this SwaggerGenOptions options)
    {
        // https://stackoverflow.com/questions/56234504/migrating-to-swashbuckle-aspnetcore-version-5
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer {token}\"",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer"
        });

        options.AddSecurityRequirement(new OpenApiSecurityRequirement()
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    },
                    Scheme = "oauth2",
                    Name = "Bearer",
                    In = ParameterLocation.Header,
                },
                new List<string>()
            }
        });

        return options;
    }

    public static SwaggerGenOptions AddTokenAuth(this SwaggerGenOptions c, string token = "token")
    {

        var schema = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.ApiKey,
            In = ParameterLocation.Query,
            Name = token,
            Description = "Authorization thru 'token' query string key"
        };

        c.AddSecurityDefinition(token, schema);

        // https://github.com/domaindrivendev/Swashbuckle.AspNetCore/issues/1425#issuecomment-632950844
        c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = token
                            }
                        }, new List<string>()
                    }
                });

        return c;
    }
}
