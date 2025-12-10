namespace server; 
static class Service
{
    public enum ServiceCategory
    {
        staying,
        activities
    }

     public enum ServiceType
    {
        hotel,
        motel,
        bedandbreakfast,
        camping,
        glamping,
        museum,
        training
        
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

            if(string.IsNullOrWhiteSpace(service.City))
                return Results.BadRequest($"City is required");

             if(!Enum.TryParse<ServiceType>(service.Type, ignoreCase: true, out var type))
            {
                return Results.BadRequest($"Invalid input: {service.Type}  type: hotel/motel/bedandbreakfast/camping/glamping");
            }

            if(!Enum.TryParse<ServiceCategory>(service.Category, ignoreCase: true, out var category))
            {
                return Results.BadRequest($"Invalid input: {service.Category}  category: hotel/activities");
            }
            string query = "INSERT INTO service(name, category, type, city) VALUES(@name, @category, @type, @city )";
            var parameters = new MySqlParameter[]
            {
            new("@name", service.Name),
            new("@category", category.ToString()),
            new("@type", type.ToString()),
            new("@city", service.City)
            };

            await MySqlHelper.ExecuteNonQueryAsync(config.db, query, parameters);

            return Results.Created("/service", service);  
      }
}


