using server.Features.Lodgings.Models;

namespace server.Features.Lodgings.Repositories
{
    public interface ILodgingRepository
    {
        Task<IEnumerable<Lodging>> GetAllLodgings();
        // Task<Lodging?> GetLodgingById(int id);
        // Task CreateLodging(Lodging lodging);
    }
}