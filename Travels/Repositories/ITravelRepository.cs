using server.Travels.Models;

namespace server.Travels.Repositories;

public interface ITravelRepository
{
    Task<IEnumerable<Travel>> GetAllTravels();
    Task CreateTravel(int userId, int transportId);
}
