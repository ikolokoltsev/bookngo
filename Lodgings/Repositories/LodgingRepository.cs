using server.Lodgings.Models;

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
        using var connection = new MySqlConnection(_config.db);
        await connection.OpenAsync();

        var query = new MySqlCommand("SELECT * FROM lodgings", connection);
        using var reader = await query.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            lodgings.Add(new Lodging
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                Name = reader.GetString(reader.GetOrdinal("Name")),
                Price = reader.GetDouble(reader.GetOrdinal("Price")),
                Address = reader.GetString(reader.GetOrdinal("Address")),
                Rating = reader.GetDouble(reader.GetOrdinal("Rating"))
            });
        }

        return lodgings;
    }

    // public Task<Lodging?> GetLodgingById(Config config, int id)
    // {

    // }
}
