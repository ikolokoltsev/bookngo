namespace server.Features.Lodgings.Models
{
    public class Lodging
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Address { get; set; }
        public required double Rating { get; set; }
        // public string? ImageUrl { get; set; }
        // TODO: Added additional info about lodging(WiFi, Parking, etc.)
        // public string Description { get; set; }
        public required double Price { get; set; }
    }
}