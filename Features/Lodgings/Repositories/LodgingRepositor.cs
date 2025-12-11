using server.Features.Lodgings.Models;
using MySql.Data.MySqlClient;
using server;
using Microsoft.Extensions.Options;

namespace server.Features.Lodgings.Repositories;

public class LodgingRepository : ILodgingRepository
{
    public async Task<IEnumerable<Lodging>> GetAllLodgings(Config config)
    {
        var lodgings = new List<Lodging>();
        using var connection = new MySqlConnection(config.db);
        await connection.OpenAsync();

        var query = new MySqlCommand("SELECT * FROM Lodgings", connection);
        using var reader = await query.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            lodgings.Add(new Lodging
            {
                Id = reader.GetInt32("Id"),
                Name = reader.GetString("Name"),
                Price = reader.GetDouble("Price"),
                Address = reader.GetString("Address"),
                Rating = reader.GetDouble("Rating")
            });
        }

        return lodgings;
    }

    // public Task<Lodging?> GetLodgingById(Config config, int id)
    // {

    // }
}
