using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;

using Moedim.Microservices.AspNetCore.Security.ApiKey.Options;
using Moedim.Microservices.AspNetCore.Security.UserStore;

namespace Microsoft.Extensions.DependencyInjection;

public static class ApiKeyAuthorizationServiceCollectionExtensions
{
    public static IServiceCollection AddApiKeyQueryStringAuthentication<TId, TStore>(
                this IServiceCollection services,
                Action<ApiKeyStringQueryAuthenticationOptions> configure)
            where TId : struct
            where TStore : IUserStoreProvider<TId>
    {
        var options = new ApiKeyStringQueryAuthenticationOptions();
        configure(options);

        services.AddUserStore<TId, TStore>();

        services.AddAuthentication()
            .AddScheme<ApiKeyStringQueryAuthenticationOptions, ApiKeyQueryStringAuthenticationHandler<TId>>(options.Scheme, configure);

        return services;
    }

    public static IServiceCollection AddApiKeyQueryStringAuthentication<TId, TStore>(
            this IServiceCollection services,
            Action<ApiKeyStringQueryAuthenticationOptions> configure,
            string sectionName,
            Action<UserStore<User<TId>>, IConfiguration>? configureStore = null)
        where TId : struct
        where TStore : IUserStoreProvider<TId>
    {
        var options = new ApiKeyStringQueryAuthenticationOptions();
        configure(options);

        services.AddInMemoryUserStore(sectionName: sectionName, configure: configureStore);

        services.AddAuthentication()
            .AddScheme<ApiKeyStringQueryAuthenticationOptions, ApiKeyQueryStringAuthenticationHandler<TId>>(options.Scheme, configure);

        return services;
    }

    public static IServiceCollection AddApiKeyHeaderAuthentication<TId, TStore>(
            this IServiceCollection services,
            Action<ApiKeyHeaderAuthenticationOptions> configure,
            string sectionName,
            Action<UserStore<User<TId>>, IConfiguration>? configureStore = null)
        where TId : struct
        where TStore : IUserStoreProvider<TId>
    {
        var options = new ApiKeyHeaderAuthenticationOptions();
        configure(options);

        services.AddInMemoryUserStore(sectionName: sectionName, configure: configureStore);

        services.AddAuthentication()
                .AddScheme<ApiKeyHeaderAuthenticationOptions, ApiKeyHeaderAuthenticationHandler<TId>>(options.Scheme, configure);

        return services;
    }

    public static IServiceCollection AddApiKeyHeaderAuthentication<TId, TStore>(
            this IServiceCollection services,
            Action<ApiKeyHeaderAuthenticationOptions> configure)
        where TId : struct
        where TStore : IUserStoreProvider<TId>
    {
        var options = new ApiKeyHeaderAuthenticationOptions();
        configure(options);

        services.AddUserStore<TId, TStore>();

        services.AddAuthentication()
                .AddScheme<ApiKeyHeaderAuthenticationOptions, ApiKeyHeaderAuthenticationHandler<TId>>(options.Scheme, configure);

        return services;
    }
}
