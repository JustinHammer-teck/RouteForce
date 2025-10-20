using RouteForce.Core.Enums;

namespace RouteForce.Core.Models;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Password { get; set; }
    public UserRole UserRole { get; set; }
    public int BusinessId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; }
    public DateTime CreatedDate { get; set; }
    public Business Business { get; set; }
}