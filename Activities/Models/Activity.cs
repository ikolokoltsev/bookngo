namespace server.Activities.Models;

public enum ActivityType
{
    Diving,
    Surfing,
    Skiing,
    Dancing,
    Hiking,
    Other
}

public enum ActivityStatus
{
    Available,
    SoldOut,
    Cancelled,
    Closed
}

public record ActivityAdditionalInfo
{
    public bool HasEquipment { get; init; }
    public bool HasInstructor { get; init; }
    public bool IsIndoor { get; init; }
}

public record ActivityAdditionalInfoPatch
{
    public bool? HasEquipment { get; init; }
    public bool? HasInstructor { get; init; }
    public bool? IsIndoor { get; init; }

    public bool HasUpdates() =>
        HasEquipment.HasValue || HasInstructor.HasValue || IsIndoor.HasValue;
}

public class Activity
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Country { get; set; }
    public required string City { get; set; }
    public required string Address { get; set; }
    public required DateTime StartTime { get; set; }
    public required DateTime EndTime { get; set; }
    public required double Price { get; set; }
    public required ActivityType Type { get; set; }
    public required ActivityStatus Status { get; set; }
    public ActivityAdditionalInfo AdditionalInfo { get; set; } = new();
    public string? Description { get; set; }
}

public record ActivityData
{
    public int Id { get; init; }
    public required string Name { get; init; }
    public required string Country { get; init; }
    public required string City { get; init; }
    public required string Address { get; init; }
    public required DateTime StartTime { get; init; }
    public required DateTime EndTime { get; init; }
    public required double Price { get; init; }
    public required ActivityType Type { get; init; }
    public required ActivityStatus Status { get; init; }
}

public record ActivityDetail
{
    public int Id { get; init; }
    public required string Name { get; init; }
    public required string Country { get; init; }
    public required string City { get; init; }
    public required string Address { get; init; }
    public required DateTime StartTime { get; init; }
    public required DateTime EndTime { get; init; }
    public required double Price { get; init; }
    public required ActivityType Type { get; init; }
    public required ActivityStatus Status { get; init; }
    public ActivityAdditionalInfo AdditionalInfo { get; init; } = new();
    public string? Description { get; init; }
}

public class ActivityUpdateRequest
{
    public string? Name { get; set; }
    public string? Country { get; set; }
    public string? City { get; set; }
    public string? Address { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public double? Price { get; set; }
    public ActivityType? Type { get; set; }
    public ActivityStatus? Status { get; set; }
    public ActivityAdditionalInfoPatch? AdditionalInfo { get; set; }
    public string? Description { get; set; }

    public bool HasUpdates() =>
        Name != null ||
        Country != null ||
        City != null ||
        Address != null ||
        StartTime.HasValue ||
        EndTime.HasValue ||
        Price.HasValue ||
        Type.HasValue ||
        Status.HasValue ||
        Description != null ||
        (AdditionalInfo != null && AdditionalInfo.HasUpdates());
}

public class ActivityFilterQuery
{
    public double? MinPrice { get; set; }
    public double? MaxPrice { get; set; }
    public ActivityType? Type { get; set; }
    public ActivityStatus? Status { get; set; }
    public string? Country { get; set; }
    public string? City { get; set; }
    public string? Address { get; set; }
    public string? SearchTerm { get; set; }
}
