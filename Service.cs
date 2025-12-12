namespace server; 
using System.Data;
static class ServiceInsertInto
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


static class ServiceBrowseAll
{
    public record Service(
        int Id,
        string Name,
        string Category,
        string Type,
        string City
    );

    public static async Task<IResult> Get(
        string? city,
        string? category,
        string? type,
        string? search,
        Config config)
    {
        var conditions = new List<string>();
        var parameters = new List<MySqlParameter>();

        if (!string.IsNullOrWhiteSpace(city))
        {
            conditions.Add("city = @city");
            parameters.Add(new("@city", city));
        }

        if (!string.IsNullOrWhiteSpace(category))
        {
            conditions.Add("category = @category");
            parameters.Add(new("@category", category));
        }

        if (!string.IsNullOrWhiteSpace(type))
        {
            conditions.Add("type = @type");
            parameters.Add(new("@type", type));
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            conditions.Add("name LIKE @search");
            parameters.Add(new("@search", $"%{search}%"));
        }

        string query = "SELECT id, name, category, type, city FROM service";

        if (conditions.Count > 0)
            query += " WHERE " + string.Join(" AND ", conditions);

        var dt = await MySqlHelper.ExecuteDatasetAsync(config.db, query, parameters.ToArray());
        var table = dt.Tables[0];

        var list = new List<Service>();

        foreach (DataRow row in table.Rows)
        {
            list.Add(new Service(
                Id: Convert.ToInt32(row["id"]),
                Name: row["name"].ToString()!,
                Category: row["category"].ToString()!,
                Type: row["type"].ToString()!,
                City: row["city"].ToString()!
            ));
        }

        return Results.Ok(list);
    }
}
