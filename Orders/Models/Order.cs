namespace server.Orders.Models;

public record OrderSummary
{
    public int Id { get; init; }
    public int UserID { get; init; }
    public double TotalAmount { get; init; }
    public DateTime CreatedAt { get; init; }
}

public record OrderLodgingItem
{
    public int LodgingID { get; init; }
    public required string Name { get; init; }
    public required string Address { get; init; }
    public double Price { get; init; }
}

public record OrderTravelItem
{
    public int TravelID { get; init; }
    public int TransportID { get; init; }
    public required string Name { get; init; }
    public DateTime DepartureTime { get; init; }
    public DateTime ArrivalTime { get; init; }
    public double Price { get; init; }
}

public record OrderActivityItem
{
    public int ActivityBookingID { get; init; }
    public int ActivityID { get; init; }
    public required string Name { get; init; }
    public DateTime StartTime { get; init; }
    public DateTime EndTime { get; init; }
    public double Price { get; init; }
}

public class OrderDetail
{
    public int Id { get; set; }
    public int UserID { get; set; }
    public double TotalAmount { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<OrderLodgingItem> Lodgings { get; set; } = new();
    public List<OrderTravelItem> Travels { get; set; } = new();
    public List<OrderActivityItem> Activities { get; set; } = new();
}

public class OrderCreateRequest
{
    public List<int> LodgingIDs { get; set; } = new();
    public List<int>? TravelIDs { get; set; }
    public List<int>? ActivityBookingIDs { get; set; }
}

public record OrderCreateResult(int OrderId, double TotalAmount);
