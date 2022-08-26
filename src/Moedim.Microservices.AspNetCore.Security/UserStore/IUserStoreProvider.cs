namespace Moedim.Microservices.AspNetCore.Security.UserStore
{
    public interface IUserStoreProvider<TId>
        where TId : struct
    {
        Task DeleteAsync(TId id, CancellationToken cancellationToken = default);

        Task<User<TId>> GetByUserNameAsync(string userName, CancellationToken cancellationToken = default);

        Task SaveAsync(User<TId> user, CancellationToken cancellationToken = default);

        Task<User<TId>> GetByApiKeyAsync(string apiKey, CancellationToken cancellationToken = default);

        Task<User<TId>> GetByIdAsync(TId id, CancellationToken cancellationToken = default);
    }
}
