using RouteForce.Application.Common;
using RouteForce.Application.Common.Interfaces;

namespace RouteForce.Application.Service;

public class UserServices
{
    private readonly IApplicationDbContext _dbContext;

    public UserServices(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<UserAuthenticationResult> SignInAsync(string inputUsername, string inputPassword)
    {
        await CheckUserAuthentication(inputUsername, inputPassword);
        
        return new UserAuthenticationResult(false, []);
    }

    private async Task<UserAuthenticationResult> CheckUserAuthentication(string inputUsername, string inputPassword)
    {
        _dbContext.Users.Where(x => x.Email == inputUsername && x.Password == inputPassword);
        return new UserAuthenticationResult(false, []);
    }
}

