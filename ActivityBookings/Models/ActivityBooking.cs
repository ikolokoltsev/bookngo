namespace server.ActivityBookings.Models;

public record ActivityBooking
{
    public int Id { get; set; }
    public int UserID { get; set; }
    public int ActivityID { get; set; }
}
