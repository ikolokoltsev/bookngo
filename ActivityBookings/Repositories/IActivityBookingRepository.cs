using server.ActivityBookings.Models;

namespace server.ActivityBookings.Repositories;

public interface IActivityBookingRepository
{
    Task<IEnumerable<ActivityBooking>> GetAllActivityBookings();
    Task CreateActivityBooking(int userId, int activityId);
}
