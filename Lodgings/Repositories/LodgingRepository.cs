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

        if (!string.IsNullOrWhiteSpace(filter.Country))
        {
            queryParts.Add("AND Country = @Country");
            parameters.Add(new MySqlParameter("@Country", filter.Country));
        }

        if (!string.IsNullOrWhiteSpace(filter.City))
        {
            queryParts.Add("AND City = @City");
            parameters.Add(new MySqlParameter("@City", filter.City));
        }

        if (!string.IsNullOrWhiteSpace(filter.Address))
        {
            queryParts.Add("AND Address = @Address");
            parameters.Add(new MySqlParameter("@Address", filter.Address));
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

    public async Task<bool> UpdateLodging(int id, LodgingUpdateRequest update)
    {
        List<string> setParts = new();
        List<MySqlParameter> parameters = new();

        if (update.Name != null)
        {
            setParts.Add("Name = @name");
            parameters.Add(new MySqlParameter("@name", update.Name));
        }

        if (update.Country != null)
        {
            setParts.Add("Country = @country");
            parameters.Add(new MySqlParameter("@country", update.Country));
        }

        if (update.City != null)
        {
            setParts.Add("City = @city");
            parameters.Add(new MySqlParameter("@city", update.City));
        }

        if (update.Address != null)
        {
            setParts.Add("Address = @address");
            parameters.Add(new MySqlParameter("@address", update.Address));
        }

        if (update.Rating.HasValue)
        {
            setParts.Add("Rating = @rating");
            parameters.Add(new MySqlParameter("@rating", update.Rating.Value));
        }

        if (update.Status.HasValue)
        {
            setParts.Add("Status = @status");
            parameters.Add(new MySqlParameter("@status", update.Status.Value.ToString()));
        }

        if (update.Description != null)
        {
            setParts.Add("description = @description");
            parameters.Add(new MySqlParameter("@description", update.Description));
        }

        if (update.Price.HasValue)
        {
            setParts.Add("Price = @price");
            parameters.Add(new MySqlParameter("@price", update.Price.Value));
        }

        if (update.AdditionalInfo?.HasWifi is bool hasWifi)
        {
            setParts.Add("has_wifi = @hasWifi");
            parameters.Add(new MySqlParameter("@hasWifi", hasWifi));
        }

        if (update.AdditionalInfo?.HasParking is bool hasParking)
        {
            setParts.Add("has_parking = @hasParking");
            parameters.Add(new MySqlParameter("@hasParking", hasParking));
        }

        if (update.AdditionalInfo?.HasPool is bool hasPool)
        {
            setParts.Add("has_pool = @hasPool");
            parameters.Add(new MySqlParameter("@hasPool", hasPool));
        }

        if (update.AdditionalInfo?.HasGym is bool hasGym)
        {
            setParts.Add("has_gym = @hasGym");
            parameters.Add(new MySqlParameter("@hasGym", hasGym));
        }

        if (setParts.Count == 0)
        {
            return false;
        }

        parameters.Add(new MySqlParameter("@id", id));
        string query = $"UPDATE lodgings SET {string.Join(", ", setParts)} WHERE Id = @id";
        int affected = await MySqlHelper.ExecuteNonQueryAsync(_config.db, query, parameters.ToArray());
        return affected > 0;
    }

    public async Task DeleteLodging(int id)
    {
        const string query = "DELETE FROM lodgings WHERE Id = @id";
        var parameters = new MySqlParameter[] { new("@id", id) };
        await MySqlHelper.ExecuteNonQueryAsync(_config.db, query, parameters);
    }
}
