namespace RouteForce.Core.Models;

public class PersonalReceiver
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string Notes { get; set; }
    public DateTime CreatedDate { get; set; }
    public int CheckpointID { get; set; }
    public int CreatedByBusinessID { get; set; }
}