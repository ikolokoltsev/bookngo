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

    public async Task<IEnumerable<Lodging>> GetFilteredLodgings(LodgingFilterQuery filter)
    {
        var lodgings = new List<Lodging>();
        var queryParts = new List<string> { "SELECT Id, Name, Price, Address, Rating, Status FROM lodgings WHERE 1=1" };
        var parameters = new List<MySqlParameter>();
        if(filter.MinPrice.HasValue)
        {
            queryParts.Add("AND Price >= @MinPrice");
            parameters.Add(new MySqlParameter("@MinPrice", filter.MinPrice.Value));
        }

        if(filter.MaxPrice.HasValue)
        {
            queryParts.Add("AND Price <= @MaxPrice");
            parameters.Add(new MySqlParameter("@MaxPrice", filter.MaxPrice.Value));
        }

         if(filter.MinRating.HasValue)
        {
            queryParts.Add("AND Rating >= @MinRating");
            parameters.Add(new MySqlParameter("@MinRating", filter.MinRating.Value));
        }

        if(filter.Status.HasValue)
        {
            queryParts.Add("AND Status = @Status");
            parameters.Add(new MySqlParameter("@Status", filter.Status.Value.ToString()));
        }

        if(!string.IsNullOrWhiteSpace(filter.SearchTerm))
        {
            queryParts.Add("AND (Name LIKE @SearchTerm OR Address LIKE @SearchTerm)");
            parameters.Add(new MySqlParameter("@SearchTerm", $"%{filter.SearchTerm}%"));
        }

        string query = string.Join(" ", queryParts);

        using var reader = await MySqlHelper.ExecuteReaderAsync(_config.db, query, parameters.ToArray());

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