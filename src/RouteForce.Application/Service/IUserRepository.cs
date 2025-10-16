using RouteForce.Core.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RouteForce.Application.Service;

public interface IUserRepository
{
    Task<User> FindByUsernameAsync(string usename);
    Task CreateUserAsync(User user);
    IEnumerable<User> GetAllUsers();
}

public class UserRepository : IUserRepository
{
    private readonly List<User> _users = new(); 

    public Task<User> FindByUsernameAsync(string username)
    {
        var user = _users.FirstOrDefault(u => u.Username == username);
        return Task.FromResult(user);
    }

    public Task CreateUserAsync(User user)
    {
        _users.Add(user);
        return Task.CompletedTask;
    }

    public IEnumerable<User> GetAllUsers() => _users;
}
