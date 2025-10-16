using RouteForce.Core.Enums;

namespace RouteForce.Core.Model;

public class User
{
    public int UserId { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string Name { get; set; } = string.Empty;
    public UserType UserType { get; set; }
    public int BusinessId { get; set; }
    public string BusinessName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; }
    public DateTime CreatedDate { get; set; }
}