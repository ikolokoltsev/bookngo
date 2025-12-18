namespace server.Bookings.Models;

public record Booking()
{
    public int UserID { get; set; }
    public int LodgingID { get; set; }
}

public record BookingInfo()
{
    public required string LodgingName { get; set; }
    public required string LodgingAddress { get; set; }
}