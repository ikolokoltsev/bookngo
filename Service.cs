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
       

      public static async Task Post(Post_Args service, Config config)
      {
            if(!Enum.TryParse<ServiceCategory>(service.Category, ignoreCase: true, out var category))
            {
                throw new ArgumentException($"Invalid category: {service.Category} ");
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
      }
}


