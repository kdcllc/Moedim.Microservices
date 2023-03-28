using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Moedim.Microservices.AspNetCore.Security.UserStore;

namespace Moedim.Microservices.UnitTest.Security
{
    public class InMemoryUserStoreProviderTests
    {
        [Fact]
        public async Task Get_Update_Delete_Successfully_Async()
        {
            var services = new ServiceCollection();

            var configBuilder = new ConfigurationBuilder();

            var dict = new Dictionary<string, string>
                {
                    { "UserStore:Users:0:Id", "1" },
                    { "UserStore:Users:0:UserName", "name1" },
                    { "UserStore:Users:0:Email", "e@e1.com" },
                    { "UserStore:Users:0:ApiKey", "key1" },
                    { "UserStore:Users:0:Roles:0", "admin" }
                };

            configBuilder.AddInMemoryCollection(dict!);
            var config = configBuilder.Build();

            services.AddSingleton<IConfiguration>(config);

            services.AddInMemoryUserStore<int>();

            var sp = services.BuildServiceProvider();

            var userStore = sp.GetRequiredService<IUserStoreProvider<int>>();

            Assert.NotNull(userStore);

            var user1 = await userStore.GetByUserNameAsync("name1", CancellationToken.None);

            Assert.Equal("name1", user1.UserName);

            user1.ApiKey = "keyChanged1";
            await userStore.SaveAsync(user1, CancellationToken.None);

            var user2 = await userStore.GetByApiKeyAsync(user1.ApiKey);
        }
    }
}
