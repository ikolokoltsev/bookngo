namespace server; 
static class Service
{
    public enum ServiceCategory
    {
        staying,
        activities
    }
    public record Post_Args(
        string Name,
        string Category,
        string Type,
        string City
    );
      public static async Task<IResult> Post(Post_Args service, Config config)
      {
            if(string.IsNullOrWhiteSpace(service.Name))
                return Results.BadRequest($"Name is required");
            

            if(string.IsNullOrWhiteSpace(service.Type))
                return Results.BadRequest($"Type is required");

            if(string.IsNullOrWhiteSpace(service.City))
                return Results.BadRequest($"City is required");

            if(!Enum.TryParse<ServiceCategory>(service.Category, ignoreCase: true, out var category))
            {
                return Results.BadRequest($"Invalid input: {service.Category}  category: staying/activities");
            }
            string query = "INSERT INTO service(name, category, type, city) VALUES(@name, @category, @type, @city )";
            var parameters = new MySqlParameter[]
            {
            new("@name", service.Name),
            new("@category", category.ToString()),
            new("@type", service.Type),
            new("@city", service.City)
            };

            await MySqlHelper.ExecuteNonQueryAsync(config.db, query, parameters);

            return Results.Created("/service", service);  
      }
}


