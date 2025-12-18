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

public record AdditionalInfo
{
    public bool HasWifi { get; init; }
    public bool HasParking { get; init; }
    public bool HasPool { get; init; }
    public bool HasGym { get; init; }
}

public class Lodging
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Country { get; set; }
    public required string City { get; set; }
    public required string Address { get; set; }
    public required double Rating { get; set; }
    public required LodgingStatus Status { get; set; }
    public AdditionalInfo AdditionalInfo { get; set; } = new();
    public string? Description { get; set; }
    // public string? 
    public required double Price { get; set; }
}

public record LodgingData
{
    public int Id { get; init; }
    public required string Name { get; init; }
    public required string Country { get; init; }
    public required string City { get; init; }
    public required string Address { get; init; }
    public required double Rating { get; init; }
    public required LodgingStatus Status { get; init; }
    public required double Price { get; init; }
}

public record LodgingDetail
{
    public int Id { get; init; }
    public required string Name { get; init; }
    public required string Country { get; init; }
    public required string City { get; init; }
    public required string Address { get; init; }
    public required double Rating { get; init; }
    public required LodgingStatus Status { get; init; }
    public AdditionalInfo AdditionalInfo { get; init; } = new();
    public string? Description { get; init; }
    public required double Price { get; init; }
}

public class LodgingFilterQuery
{
      public double? MinPrice { get; set; }
      public double? MaxPrice { get; set; }
      public double? MinRating { get; set; }  
      public LodgingStatus? Status { get; set; }
      public string? SearchTerm { get; set; }
}
