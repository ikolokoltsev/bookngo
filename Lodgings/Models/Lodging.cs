namespace server.Lodgings.Models;

// TODO: replace enum, because parsing enums is pain
public enum LodgingStatus
{
    Available,
    Booked,
    Unavailable,
    UnderMaintenance,
    PendingApproval
}

// TODO: Complete additional info about lodging(WiFi, Parking, etc.)

public record AdditionalInfo(
    bool WiFi,
    bool Parking,
    bool Pool,
    bool Gym
);

public class Lodging
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Address { get; set; }
    public required double Rating { get; set; }
    public required LodgingStatus Status { get; set; }
    // public required AdditionalInfo AdditionalInfo { get; set; }
    // public string? ImageUrl { get; set; }
    // public string Description { get; set; }
    public required double Price { get; set; }
}
