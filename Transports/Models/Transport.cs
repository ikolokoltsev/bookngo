namespace server.Transports.Models;

public enum TransportType
{
    Flight,
    Train,
    Bus,
    Ferry,
    Other
}

public enum TransportStatus
{
    Available,
    SoldOut,
    Cancelled,
    Delayed
}

public record TransportAdditionalInfo
{
    public bool HasWifi { get; init; }
    public bool HasFood { get; init; }
    public bool HasPremiumClass { get; init; }
}

public class Transport
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Origin { get; set; }
    public required string Destination { get; set; }
    public required DateTime DepartureTime { get; set; }
    public required DateTime ArrivalTime { get; set; }
    public required double Price { get; set; }
    public required TransportType Type { get; set; }
    public required TransportStatus Status { get; set; }
    public TransportAdditionalInfo Amenities { get; set; } = new();
    public string? Description { get; set; }
}

public record TransportData
{
    public int Id { get; init; }
    public required string Name { get; init; }
    public required string Origin { get; init; }
    public required string Destination { get; init; }
    public required DateTime DepartureTime { get; init; }
    public required DateTime ArrivalTime { get; init; }
    public required double Price { get; init; }
    public required TransportType Type { get; init; }
    public required TransportStatus Status { get; init; }
}

public record TransportDetail
{
    public int Id { get; init; }
    public required string Name { get; init; }
    public required string Origin { get; init; }
    public required string Destination { get; init; }
    public required DateTime DepartureTime { get; init; }
    public required DateTime ArrivalTime { get; init; }
    public required double Price { get; init; }
    public required TransportType Type { get; init; }
    public required TransportStatus Status { get; init; }
    public TransportAdditionalInfo Amenities { get; init; } = new();
    public string? Description { get; init; }
}

public class TransportFilterQuery
{
    public double? MinPrice { get; set; }
    public double? MaxPrice { get; set; }
    public TransportType? Type { get; set; }
    public TransportStatus? Status { get; set; }
    public string? SearchTerm { get; set; }
}
