using server.Lodgings.Models;

namespace server.Lodgings.Repositories;

public interface ILodgingRepository
{
    Task<IEnumerable<Lodging>> GetAllLodgings();
    // Task<Lodging?> GetLodgingById(int id);
    // Task CreateLodging(Lodging lodging);\

    Task<IEnumerable<Lodging>> GetFilteredLodgings(LodgingFilterQuery fillter);
}

