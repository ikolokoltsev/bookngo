using server.Lodgings.Models;

namespace server.Lodgings.Repositories;

public class LodgingRepository : ILodgingRepository
{
    private readonly Config _config;

    public LodgingRepository(Config config)
    {
        _config = config;
    }

    public async Task<IEnumerable<LodgingData>> GetAllLodgings(LodgingFilterQuery filter)
    {
        List<LodgingData> lodgings = new List<LodgingData>();
        List<string> queryParts = new List<string> { "SELECT Id, Name, Price, Address, Rating, Status FROM lodgings WHERE 1=1" };
        List<MySqlParameter> parameters = new List<MySqlParameter>();
        if (filter.MinPrice.HasValue)
        {
            queryParts.Add("AND Price >= @MinPrice");
            parameters.Add(new MySqlParameter("@MinPrice", filter.MinPrice.Value));
        }

        if (filter.MaxPrice.HasValue)
        {
            queryParts.Add("AND Price <= @MaxPrice");
            parameters.Add(new MySqlParameter("@MaxPrice", filter.MaxPrice.Value));
        }

        if (filter.MinRating.HasValue)
        {
            queryParts.Add("AND Rating >= @MinRating");
            parameters.Add(new MySqlParameter("@MinRating", filter.MinRating.Value));
        }

        if (filter.Status.HasValue)
        {
            queryParts.Add("AND Status = @Status");
            parameters.Add(new MySqlParameter("@Status", filter.Status.Value.ToString()));
        }

        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
        {
            queryParts.Add("AND (Name LIKE @SearchTerm OR Address LIKE @SearchTerm)");
            parameters.Add(new MySqlParameter("@SearchTerm", $"%{filter.SearchTerm}%"));
        }

        string query = string.Join(" ", queryParts);

        using var reader = await MySqlHelper.ExecuteReaderAsync(_config.db, query, parameters.ToArray());

        while (await reader.ReadAsync())
        {
            lodgings.Add(new LodgingData
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Price = reader.GetDouble(2),
                Address = reader.GetString(3),
                Rating = reader.GetDouble(4),
                Status = Enum.Parse<LodgingStatus>(reader.GetString(5))
            });
        }
        return lodgings;
    }

    public async Task<LodgingDetail?> GetLodgingById(int id)
    {
        const string query = """
                             SELECT Id, Name, Price, Address, Rating, Status, description,
                                    has_wifi, has_parking, has_pool, has_gym
                             FROM lodgings
                             WHERE Id = @id
                             """;
        var parameters = new MySqlParameter[] { new("@id", id) };
        using var reader = await MySqlHelper.ExecuteReaderAsync(_config.db, query, parameters);
        if (await reader.ReadAsync())
        {
            return new LodgingDetail
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Price = reader.GetDouble(2),
                Address = reader.GetString(3),
                Rating = reader.GetDouble(4),
                Status = Enum.Parse<LodgingStatus>(reader.GetString(5)),
                AdditionalInfo = new AdditionalInfo
                {
                    HasWifi = reader.GetBoolean(7),
                    HasParking = reader.GetBoolean(8),
                    HasPool = reader.GetBoolean(9),
                    HasGym = reader.GetBoolean(10)
                },
                Description = reader.IsDBNull(6) ? null : reader.GetString(6)
            };
        }
        return null;
    }

    public async Task CreateLodging(Lodging lodging)
    {
        const string query = """
                             INSERT INTO lodgings
                             (Name, Price, Address, Rating, Status, has_wifi, has_parking, has_pool, has_gym, description)
                             VALUES(@name, @price, @address, @rating, @status, @hasWifi, @hasParking, @hasPool, @hasGym, @description)
                             """;
        var additionalInfo = lodging.AdditionalInfo ?? new AdditionalInfo();
        var parameters = new MySqlParameter[]
        {
            new("@name", lodging.Name),
            new("@price", lodging.Price),
            new("@address", lodging.Address),
            new("@rating", lodging.Rating),
            new("@status", lodging.Status.ToString()),
            new("@hasWifi", additionalInfo.HasWifi),
            new("@hasParking", additionalInfo.HasParking),
            new("@hasPool", additionalInfo.HasPool),
            new("@hasGym", additionalInfo.HasGym),
            new("@description", lodging.Description ?? (object)DBNull.Value)
        };
        await MySqlHelper.ExecuteNonQueryAsync(_config.db, query, parameters);
    }
}
