using RouteForce.Core.Enums;

namespace RouteForce.Core.Models;

public class User
{
    public string UserId { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public UserRole UserRole { get; set; }
    public int BusinessId { get; set; }
    public string BusinessName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; }
    public DateTime CreatedDate { get; set; }
}