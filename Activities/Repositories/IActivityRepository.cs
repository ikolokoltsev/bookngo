using server.Activities.Models;

namespace server.Activities.Repositories;

public interface IActivityRepository
{
    Task<IEnumerable<ActivityData>> GetAllActivities(ActivityFilterQuery filter);
    Task<ActivityDetail?> GetActivityById(int id);
    Task CreateActivity(Activity activity);
    Task<bool> UpdateActivity(int id, ActivityUpdateRequest update);
    Task DeleteActivity(int id);
}
