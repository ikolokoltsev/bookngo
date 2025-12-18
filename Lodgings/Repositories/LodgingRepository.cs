using server.Lodgings.Models;
using server.Users.Repositories;

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
        List<string> queryParts = new List<string> { "SELECT Id, Name, Price, Country, City, Address, Rating, Status FROM lodgings WHERE 1=1" };
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
            queryParts.Add("AND (Name LIKE @SearchTerm OR Country LIKE @SearchTerm OR City LIKE @SearchTerm OR Address LIKE @SearchTerm)");
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
                Country = reader.GetString(3),
                City = reader.GetString(4),
                Address = reader.GetString(5),
                Rating = reader.GetDouble(6),
                Status = Enum.Parse<LodgingStatus>(reader.GetString(7))
            });
        }
        return lodgings;
    }

    public async Task<LodgingDetail?> GetLodgingById(int id)
    {
        const string query = """
                             SELECT Id, Name, Price, Country, City, Address, Rating, Status, description,
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
                Country = reader.GetString(3),
                City = reader.GetString(4),
                Address = reader.GetString(5),
                Rating = reader.GetDouble(6),
                Status = Enum.Parse<LodgingStatus>(reader.GetString(7)),
                AdditionalInfo = new LodgingAdditionalInfo
                {
                    HasWifi = reader.GetBoolean(9),
                    HasParking = reader.GetBoolean(10),
                    HasPool = reader.GetBoolean(11),
                    HasGym = reader.GetBoolean(12)
                },
                Description = reader.IsDBNull(8) ? null : reader.GetString(8)
            };
        }
        return null;
    }

    public async Task CreateLodging(Lodging lodging)
    {
        const string query = """
                                INSERT INTO lodgings
                                (Name, Price, Country, City, Address, Rating, Status, has_wifi, has_parking, has_pool, has_gym, description)
                                VALUES(@name, @price, @country, @city, @address, @rating, @status, @hasWifi, @hasParking, @hasPool, @hasGym, @description)
                                """;
        var additionalInfo = lodging.AdditionalInfo ?? new LodgingAdditionalInfo();
        var parameters = new MySqlParameter[]
        {
                new("@name", lodging.Name),
                new("@price", lodging.Price),
                new("@country", lodging.Country),
                new("@city", lodging.City),
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

    public async Task DeleteLodging(int id)
    {
        const string query = "DELETE FROM lodgings WHERE Id = @id";
        var parameters = new MySqlParameter[] { new("@id", id) };
        await MySqlHelper.ExecuteNonQueryAsync(_config.db, query, parameters);
    }
}
