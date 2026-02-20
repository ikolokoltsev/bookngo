namespace server.Travels.Models;

public record Travel
{
    public int Id { get; set; }
    public int UserID { get; set; }
    public int TransportID { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
