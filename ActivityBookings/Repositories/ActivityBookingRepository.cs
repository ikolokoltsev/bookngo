using server.ActivityBookings.Models;

namespace server.ActivityBookings.Repositories;

public class ActivityBookingRepository : IActivityBookingRepository
{
    private readonly Config _config;

    public ActivityBookingRepository(Config config)
    {
        _config = config;
    }

    public async Task<IEnumerable<ActivityBooking>> GetAllActivityBookings()
    {
        var bookings = new List<ActivityBooking>();

        const string query = "SELECT Id, UserID, ActivityID FROM activity_bookings";
        using var reader = await MySqlHelper.ExecuteReaderAsync(_config.db, query);

        while (await reader.ReadAsync())
        {
            bookings.Add(new ActivityBooking
            {
                Id = reader.GetInt32(0),
                UserID = reader.GetInt32(1),
                ActivityID = reader.GetInt32(2)
            });
        }

        return bookings;
    }

    public async Task CreateActivityBooking(int userId, int activityId)
    {
        const string query = "INSERT INTO activity_bookings (UserID, ActivityID) VALUES (@userId, @activityId)";
        var parameters = new MySqlParameter[]
        {
            new("@userId", userId),
            new("@activityId", activityId)
        };

        await MySqlHelper.ExecuteNonQueryAsync(_config.db, query, parameters);
    }
}
