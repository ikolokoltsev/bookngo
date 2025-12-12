using server.Lodgings.Models;
using System.Text.Json;

namespace server.Lodgings.Repositories;

public class LodgingRepository : ILodgingRepository
{
    private readonly Config _config;

    public LodgingRepository(Config config)
    {
        _config = config;
    }

    public async Task<IEnumerable<Lodging>> GetAllLodgings()
    {
        var lodgings = new List<Lodging>();

        const string query = "SELECT Id, Name, Price, Address, Rating, Status FROM lodgings";
        using var reader = await MySqlHelper.ExecuteReaderAsync(_config.db, query);

        while (await reader.ReadAsync())
        {
            lodgings.Add(new Lodging
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
}