using Microsoft.OpenApi.Models;

using Swashbuckle.AspNetCore.SwaggerGen;

namespace Microsoft.Extensions.DependencyInjection;

public static class SwaggerGenOptionsExtensions
{
    public static SwaggerGenOptions AddBearerAuth(this SwaggerGenOptions options)
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

    public static SwaggerGenOptions AddApiKeyQueryAuth(
        this SwaggerGenOptions options,
        string apiKeyName = "token")
    {
        var schema = new OpenApiSecurityScheme
        {
            Name = apiKeyName,
            Type = SecuritySchemeType.ApiKey,
            In = ParameterLocation.Query,
            Description = $"ApiKey Authorization with custom: '{apiKeyName}' query string key scheme."
        };

        options.AddSecurityDefinition(apiKeyName, schema);

        options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = apiKeyName
                            }
                        }, new List<string>()
                    }
                });

        return options;
    }

    public static SwaggerGenOptions AddApiKeyHeaderAuth(this SwaggerGenOptions options, string apiKey = "apiKey")
    {
        var schema = new OpenApiSecurityScheme
        {
            Name = apiKey,
            Type = SecuritySchemeType.ApiKey,
            Scheme = apiKey,
            In = ParameterLocation.Header,
            Description = $"ApiKey Authorization with custom header: '{apiKey}' scheme."
        };

        options.AddSecurityDefinition(apiKey, schema);

        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = apiKey
                        }
                    },
                    new string[] { }
            }
        });

        return options;
    }

    public static SwaggerGenOptions AddBasicHeaderAuth(this SwaggerGenOptions options, string token = "token")
    {
        options.AddSecurityDefinition("basic", new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "basic",
            In = ParameterLocation.Header,
            Description = "Basic Authorization header scheme."
        });

        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "basic"
                        }
                    },
                    new string[] { }
            }
        });

        return options;
    }
}
