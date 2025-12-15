using server.Lodgings.Models;

namespace server.Lodgings.Repositories;

public interface ILodgingRepository
{
    Task<IEnumerable<LodgingData>> GetAllLodgings();
    Task<LodgingDetail?> GetLodgingById(int id);
    Task CreateLodging(Lodging lodging);
}
