namespace Moedim.Microservices.AspNetCore.Security.UserStore;

public class UserStore<TUser> where TUser : class
{
    public List<TUser> Users { get; set; } = new List<TUser>();
}
