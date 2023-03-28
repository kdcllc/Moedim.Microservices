using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

using Moedim.Microservices.AspNetCore.Security.Jwt.Options;
using Moedim.Microservices.AspNetCore.Security.Jwt.Services;
using Moedim.Microservices.AspNetCore.Security.Jwt.Services.Impl;
using Moedim.Microservices.AspNetCore.Security.UserStore;

namespace Microsoft.Extensions.DependencyInjection;

public static class JwtTokenServiceCollectionExtensions
{
    /// <summary>
    /// Add Jwt Authentication with <see cref="DefaultUserService{TId}"/> that always returns true.
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    /// <typeparam name="TStore"></typeparam>
    /// <param name="services">The DI services.</param>
    /// <param name="configure"></param>
    /// <returns></returns>
    public static IServiceCollection AddDefaultJwtAuthentication<TId, TStore>(
        this IServiceCollection services,
        Action<AuthenticationOptions>? configure = null)
            where TId : struct
            where TStore : IUserStoreProvider<TId>
    {
        return services
            .AddJwtAuthentication<DefaultUserService<TId>, TId, TStore>(options => configure?.Invoke(options));
    }

    /// <summary>
    /// Adds Jwt Authentication with custom <see cref="IUserService{TId}"/> implementation.
    /// </summary>
    /// <typeparam name="TUserService"></typeparam>
    /// <typeparam name="TId"></typeparam>
    /// <typeparam name="TStore"></typeparam>
    /// <param name="services"></param>
    /// <param name="configureAuthOptions"></param>
    /// <param name="jwtSectionName"></param>
    /// <param name="configureJwt"></param>
    /// <param name="userStoreSectionName"></param>
    /// <param name="configureStore"></param>
    /// <returns></returns>
    public static IServiceCollection AddJwtAuthentication<TUserService, TId, TStore>(
                    this IServiceCollection services,
                    Action<AuthenticationOptions> configureAuthOptions,
                    string jwtSectionName = nameof(JwtTokenAuthOptions),
                    Action<JwtTokenAuthOptions, IConfiguration>? configureJwt = default,
                    string userStoreSectionName = "UserStore",
                    Action<UserStore<User<TId>>, IConfiguration>? configureStore = null)
            where TUserService : IUserService<TId>
            where TId : struct
            where TStore : IUserStoreProvider<TId>
    {
        services.AddTransient<IClaimsTransformation, RoleClaimsTransformation>();

        services.AddChangeTokenOptions<JwtTokenAuthOptions>(
            sectionName: jwtSectionName,
            configureAction: (o, c) => configureJwt?.Invoke(o, c));

        services.AddSingleton<IPostConfigureOptions<JwtBearerOptions>, PostConfigureJwtBearerOptions>();

        services.AddAuthentication(configureAuthOptions)
                .AddJwtBearer();

        services.TryAdd(ServiceDescriptor.Describe(
                    typeof(IUserService<TId>),
                    typeof(TUserService),
                    ServiceLifetime.Scoped));

        services.AddScoped<IJwtTokenAuthenticationService<TId>, JwtTokenAuthenticationService<TId>>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();

        services.AddInMemoryUserStore(sectionName: userStoreSectionName, configure: configureStore);

        services.AddControllers();

        return services;
    }
}
