using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;

using Moedim.Microservices.AspNetCore.Security.UserStore;

namespace Microsoft.Extensions.DependencyInjection;

public static class UserStoreServiceCollectionExtensions
{
    public static IServiceCollection AddUserStore<TId, TStore>(this IServiceCollection services)
        where TId : struct
        where TStore : IUserStoreProvider<TId>
    {
        services.TryAdd(ServiceDescriptor.Describe(
            typeof(IUserStoreProvider<TId>),
            typeof(TStore),
            ServiceLifetime.Scoped));
        return services;
    }

    public static IServiceCollection AddInMemoryUserStore<TId>(
        this IServiceCollection services,
        string sectionName = "UserStore",
        Action<UserStore<User<TId>>, IConfiguration>? configure = null)
         where TId : struct
    {
        services.AddChangeTokenOptions<UserStore<User<TId>>>(
                                    sectionName,
                                    nameof(InMemoryUserStoreProvider<TId>),
                                    (o, c) => configure?.Invoke(o, c));

        services.AddUserStore<TId, InMemoryUserStoreProvider<TId>>();

        return services;
    }
}
