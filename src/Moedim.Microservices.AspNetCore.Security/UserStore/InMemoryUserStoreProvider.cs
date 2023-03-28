using System.Collections.Concurrent;

using Microsoft.Extensions.Options;

namespace Moedim.Microservices.AspNetCore.Security.UserStore;

public class InMemoryUserStoreProvider<TId> : IUserStoreProvider<TId>
    where TId : struct
{
    private readonly ConcurrentDictionary<TId, User<TId>> _store = new ConcurrentDictionary<TId, User<TId>>();
    private UserStore<User<TId>> _options;

    public InMemoryUserStoreProvider(
        IOptionsMonitor<UserStore<User<TId>>> optionsMonitor)
    {
        var namedOption = nameof(InMemoryUserStoreProvider<TId>);

        _options = optionsMonitor.Get(namedOption);

        optionsMonitor.OnChange((o, n) =>
        {
            if (n == namedOption)
            {
                _options = o;
                UpdateUsers();
            }
        });

        UpdateUsers();
    }

    public Task DeleteAsync(TId id, CancellationToken cancellationToken = default)
    {
        Task.Run(() =>
        {
            if (!_store.TryRemove(id, out var user))
            {
                throw new ApplicationException("User Not found");
            }
        });

        return Task.CompletedTask;
    }

    public Task<User<TId>> GetByApiKeyAsync(string apiKey, CancellationToken cancellationToken = default)
    {
        var user = _store.Select(x => x.Value).FirstOrDefault(q => q.ApiKey == apiKey);

        if (user == null)
        {
            throw new ApplicationException("User Not found");
        }

        return Task.FromResult(user);
    }

    public Task<User<TId>> GetByIdAsync(TId id, CancellationToken cancellationToken = default)
    {
        if (_store.TryGetValue(id, out var user))
        {
            return Task.FromResult(user);
        }

        throw new ApplicationException("User Not found");
    }

    public Task<User<TId>> GetByUserNameAsync(string userName, CancellationToken cancellationToken = default)
    {
        var user = _store.Select(x => x.Value).FirstOrDefault(q => q.UserName == userName);

        if (user == null)
        {
            throw new ApplicationException("User Not found");
        }

        return Task.FromResult(user);
    }

    public Task SaveAsync(User<TId> user, CancellationToken cancellationToken = default)
    {
        Task.Run(() =>
        {
            var result = _store.AddOrUpdate(user.Id, user, (i, u) => user);
        });

        return Task.CompletedTask;
    }

    private void UpdateUsers()
    {
        foreach (var user in _options.Users)
        {
            _store.AddOrUpdate(user.Id, i => user, (i, u) => user);
        }
    }
}
