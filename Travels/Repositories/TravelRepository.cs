using server.Travels.Models;

namespace server.Travels.Repositories;

public class TravelRepository : ITravelRepository
{
    private readonly Config _config;

    public TravelRepository(Config config)
    {
        _config = config;
    }

    public async Task<IEnumerable<Travel>> GetAllTravels()
    {
        var bookings = new List<Travel>();

        const string query = "SELECT Id, UserID, TransportID, created_at, updated_at FROM travels";
        using var reader = await MySqlHelper.ExecuteReaderAsync(_config.db, query);

        while (await reader.ReadAsync())
        {
            bookings.Add(new Travel
            {
                Id = reader.GetInt32(0),
                UserID = reader.GetInt32(1),
                TransportID = reader.GetInt32(2),
                CreatedAt = reader.GetDateTime(3),
                UpdatedAt = reader.GetDateTime(4)
            });
        }

        return bookings;
    }

    public async Task CreateTravel(int userId, int transportId)
    {
        const string query = "INSERT INTO travels (UserID, TransportID) VALUES (@userId, @transportId)";
        var parameters = new MySqlParameter[]
        {
            new("@userId", userId),
            new("@transportId", transportId)
        };

        await MySqlHelper.ExecuteNonQueryAsync(_config.db, query, parameters);
    }
}
