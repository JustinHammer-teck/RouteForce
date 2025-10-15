using RouteForce.Core.Model;
using System.Collections.Generic;

namespace RouteForce.Application.Service;

public interface IUserRepository
{
    User FindByUserName(string username);
    void CreateUser(User user);
    IEnumerable<User> GetAllUsers();
}