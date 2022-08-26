using System.Reflection;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Models;

using Moedim.Microservices.AspNetCore.Swagger;

using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Microsoft.Extensions.DependencyInjection;

public static class MicroservicesSwaggerBuilderExtensions
{
    public static IMicroservicesSwaggerBuilder AddSwagger(
        this IServiceCollection services,
        bool enableSwagerVersionSupport)
    {
        var builder = new MicroservicesSwaggerBuilder(services, enableSwagerVersionSupport);

        builder.Services.AddControllers();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();

        if (enableSwagerVersionSupport)
        {
            // full controllers
            builder.Services.AddVersionedApiExplorer();
        }

        builder.Services.AddSwaggerGen();

        return builder;
    }

    /// <summary>
    /// <para>Adds AspNetCore Versionsing support for web apis.</para>
    /// <para>This can be added without Swagger support.</para>
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configureApiExplorer"></param>
    /// <param name="configureApiVersioning"></param>
    /// <returns></returns>
    public static IMicroservicesSwaggerBuilder ConfigureVersioning(
        this IMicroservicesSwaggerBuilder builder,
        Action<ApiExplorerOptions>? configureApiExplorer = null,
        Action<ApiVersioningOptions>? configureApiVersioning = null)
    {
        builder.Services.AddOptions<ApiExplorerOptions>()
             .Configure(options =>
             {
                 // add the versioned api explorer, which also adds IApiVersionDescriptionProvider service
                 // note: the specified format code will format the version as "'v'major[.minor][-status]"
                 options.GroupNameFormat = "'v'VVV";

                 // note: this option is only necessary when versioning by url segment. the SubstitutionFormat
                 // can also be used to control the format of the API version in route templates
                 options.SubstituteApiVersionInUrl = true;

                 configureApiExplorer?.Invoke(options);
             });

        builder.Services.AddApiVersioning(options =>
        {
            // reporting api versions will return the headers "api-supported-versions" and "api-deprecated-versions"
            options.ReportApiVersions = true;
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.AssumeDefaultVersionWhenUnspecified = true;

            configureApiVersioning?.Invoke(options);
        });

        return builder;
    }

    public static IMicroservicesSwaggerBuilder ConfigureSwaggerGen<T>(
        this IMicroservicesSwaggerBuilder builder,
        Action<SwaggerGenOptions, IServiceProvider> configureGen,
        string? assemblyName = null) where T : class
    {
        if (builder.EnableSwagerVersionSupport)
        {
            builder.Services.AddOptions<SwaggerGenOptions>()
                .Configure<IServiceProvider>((options, sp) =>
                {
                    options.EnableAnnotations();

                    var config = sp.GetRequiredService<IConfiguration>();

                    var provider = sp.GetRequiredService<IApiVersionDescriptionProvider>();
                    var appliationName = GetApplicationName<T>(assemblyName, config);

                    foreach (var description in provider.ApiVersionDescriptions)
                    {
                        options.SwaggerDoc(
                            description.GroupName,
                            CreateInfoForApiVersion(description, appliationName!));
                    }

                    options.OperationFilter<SwaggerDefaultValues>();

                    // https://github.com/domaindrivendev/Swashbuckle/issues/142
                    options.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());

                    configureGen?.Invoke(options, sp);
                });
        }
        else
        {
            builder.Services.AddOptions<SwaggerGenOptions>()
                .Configure<IServiceProvider>((options, sp) =>
                {
                    options.EnableAnnotations();

                    options.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());

                    configureGen?.Invoke(options, sp);
                });
        }

        return builder;
    }

    public static IMicroservicesSwaggerBuilder ConfigureUIOptions(
        this IMicroservicesSwaggerBuilder builder,
        Action<SwaggerUIOptions>? configureUi = null)
    {
        if (builder.EnableSwagerVersionSupport)
        {
            builder.Services.AddOptions<SwaggerUIOptions>()
                .Configure<IApiVersionDescriptionProvider>((options, provider) =>
                {
                    foreach (var description in provider.ApiVersionDescriptions)
                    {
                        options.SwaggerEndpoint(
                             $"/swagger/{description.GroupName}/swagger.json",
                             description.GroupName.ToUpperInvariant());
                    }

                    configureUi?.Invoke(options);
                });
        }
        else
        {
            builder.Services.AddOptions<SwaggerUIOptions>()
                    .Configure(options =>
                    {
                        configureUi?.Invoke(options);
                    });
        }

        return builder;
    }

    public static IMicroservicesSwaggerBuilder ConfigureSwaggerXmlDocs<T>(
        this IMicroservicesSwaggerBuilder builder,
        string? assemblyName = null) where T : class
    {
        builder.Services.AddOptions<SwaggerGenOptions>()
        .PostConfigure<IConfiguration>((options, config) =>
        {
            options.EnableAnnotations();

            var appliationName = GetApplicationName<T>(assemblyName, config);

            if (appliationName != null)
            {
                options.IncludeXmlComments(GetXmlDocPath(appliationName));
            }
        });

        return builder;
    }

    private static string? GetApplicationName<T>(
        string? assemblyName,
        IConfiguration config) where T : class
    {
        var appName = assemblyName ?? typeof(T).GetTypeInfo().Assembly.GetName().Name;

        return appName ?? config[WebHostDefaults.ApplicationKey];
    }

    private static string GetXmlDocPath(string appName)
    {
        if (appName.Contains(','))
        {
            // if app name is the full assembly name, just grab the short name part
            appName = appName.Substring(0, appName.IndexOf(','));
        }

        return Path.Combine(AppContext.BaseDirectory, $"{appName}.xml");
    }

    private static OpenApiInfo CreateInfoForApiVersion(ApiVersionDescription description, string appliationName)
    {
        var info = new OpenApiInfo
        {
            Title = $"{appliationName} API {description.ApiVersion}",
            Version = description.ApiVersion.ToString()
        };

        if (description.IsDeprecated)
        {
            info.Description += " This API version has been deprecated.";
        }

        return info;
    }
}
