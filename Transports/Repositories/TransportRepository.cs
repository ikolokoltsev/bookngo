using server.Transports.Models;

namespace server.Transports.Repositories;

public class TransportRepository : ITransportRepository
{
    private readonly Config _config;

    public TransportRepository(Config config)
    {
        _config = config;
    }

    public async Task<IEnumerable<TransportData>> GetAllTransports(TransportFilterQuery filter)
    {
        List<TransportData> transports = new();
        List<string> queryParts = new()
        {
            "SELECT Id, Name, Origin, Destination, departure_time, arrival_time, Price, transport_type, Status FROM transports WHERE 1=1"
        };
        List<MySqlParameter> parameters = new();

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

        if (filter.Type.HasValue)
        {
            queryParts.Add("AND transport_type = @Type");
            parameters.Add(new MySqlParameter("@Type", filter.Type.Value.ToString()));
        }

        if (filter.Status.HasValue)
        {
            queryParts.Add("AND Status = @Status");
            parameters.Add(new MySqlParameter("@Status", filter.Status.Value.ToString()));
        }

        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
        {
            queryParts.Add("AND (Name LIKE @SearchTerm OR Origin LIKE @SearchTerm OR Destination LIKE @SearchTerm)");
            parameters.Add(new MySqlParameter("@SearchTerm", $"%{filter.SearchTerm}%"));
        }

        string query = string.Join(" ", queryParts);

        using var reader = await MySqlHelper.ExecuteReaderAsync(_config.db, query, parameters.ToArray());
        while (await reader.ReadAsync())
        {
            transports.Add(new TransportData
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Origin = reader.GetString(2),
                Destination = reader.GetString(3),
                DepartureTime = reader.GetDateTime(4),
                ArrivalTime = reader.GetDateTime(5),
                Price = reader.GetDouble(6),
                Type = Enum.Parse<TransportType>(reader.GetString(7)),
                Status = Enum.Parse<TransportStatus>(reader.GetString(8))
            });
        }

        return transports;
    }

    public async Task<TransportDetail?> GetTransportById(int id)
    {
        const string query = """
                             SELECT Id, Name, Origin, Destination, departure_time, arrival_time, Price, transport_type, Status,
                                    has_wifi, has_food, has_premium_class, description
                             FROM transports
                             WHERE Id = @id
                             """;
        var parameters = new MySqlParameter[] { new("@id", id) };

        using var reader = await MySqlHelper.ExecuteReaderAsync(_config.db, query, parameters);
        if (await reader.ReadAsync())
        {
            return new TransportDetail
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Origin = reader.GetString(2),
                Destination = reader.GetString(3),
                DepartureTime = reader.GetDateTime(4),
                ArrivalTime = reader.GetDateTime(5),
                Price = reader.GetDouble(6),
                Type = Enum.Parse<TransportType>(reader.GetString(7)),
                Status = Enum.Parse<TransportStatus>(reader.GetString(8)),
                Amenities = new TransportAdditionalInfo
                {
                    HasWifi = reader.GetBoolean(9),
                    HasFood = reader.GetBoolean(10),
                    HasPremiumClass = reader.GetBoolean(11)
                },
                Description = reader.IsDBNull(12) ? null : reader.GetString(12)
            };
        }

        return null;
    }

    public async Task CreateTransport(Transport transport)
    {
        const string query = """
                             INSERT INTO transports
                             (Name, Origin, Destination, departure_time, arrival_time, Price, transport_type, Status,
                              has_wifi, has_food, has_premium_class, description)
                             VALUES(@name, @origin, @destination, @departureTime, @arrivalTime, @price, @type, @status,
                                    @hasWifi, @hasFood, @hasPremiumClass, @description)
                             """;
        var amenities = transport.Amenities ?? new TransportAdditionalInfo();
        var parameters = new MySqlParameter[]
        {
            new("@name", transport.Name),
            new("@origin", transport.Origin),
            new("@destination", transport.Destination),
            new("@departureTime", transport.DepartureTime),
            new("@arrivalTime", transport.ArrivalTime),
            new("@price", transport.Price),
            new("@type", transport.Type.ToString()),
            new("@status", transport.Status.ToString()),
            new("@hasWifi", amenities.HasWifi),
            new("@hasFood", amenities.HasFood),
            new("@hasPremiumClass", amenities.HasPremiumClass),
            new("@description", transport.Description ?? (object)DBNull.Value)
        };

        await MySqlHelper.ExecuteNonQueryAsync(_config.db, query, parameters);
    }

    public async Task DeleteTransport(int id)
    {
        const string query = "DELETE FROM transports WHERE Id = @id";
        var parameters = new MySqlParameter[] { new("@id", id) };
        await MySqlHelper.ExecuteNonQueryAsync(_config.db, query, parameters);
    }
}
