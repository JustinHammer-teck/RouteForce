namespace RouteForce.Core.Model;

public class Business
{
    public int BusinessId { get; set; }
    public string BusinessName { get; set; }
    public int TaxId { get; set; }
    public string ContactEmail { get; set; }
    public string ContactPhone { get; set; }
    public DateTime CreatedDate { get; set; }
}