namespace server.Bookings.Models;

public record Booking()
{
    public int UserID { get; set; }
    public int LodgingID { get; set; }
}