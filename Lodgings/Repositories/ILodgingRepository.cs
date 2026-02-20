using server.Lodgings.Models;

namespace server.Lodgings.Repositories;

public interface ILodgingRepository
{
    Task<IEnumerable<LodgingData>> GetAllLodgings(LodgingFilterQuery filter);
    Task<LodgingDetail?> GetLodgingById(int id);
    Task CreateLodging(Lodging lodging);
    Task<bool> UpdateLodging(int id, LodgingUpdateRequest update);
    Task DeleteLodging(int id);
}
