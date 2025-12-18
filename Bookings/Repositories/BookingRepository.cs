using server.Bookings.Models;
using server.Bookings.Controllers;

namespace server.Bookings.Repositories;

public class BookingRepository : IBookingRepository
{

    private readonly Config _config;

    public BookingRepository(Config config)
    {
        _config = config;
    }

    public async Task<IEnumerable<Booking>> GetAllBookings()
    {
        var bookings = new List<Booking>();

        const string query = "SELECT UserID, LodgingID FROM bookings";
        using var reader = await MySqlHelper.ExecuteReaderAsync(_config.db, query);

        while (await reader.ReadAsync())
        {
            bookings.Add(new Booking
            {
                UserID = reader.GetInt32(0),
                LodgingID = reader.GetInt32(1)
            });
        }

        return bookings;
    }

    public async Task<IEnumerable<BookingInfo>> GetUserBookings(HttpContext ctx)
    {
        var bookingInfos = new List<BookingInfo>();

        if(ctx.Session.IsAvailable)
        {
            if(ctx.Session.Keys.Contains("user_id"))
            {
                // const string query = "SELECT LodgingID FROM bookings WHERE LodgingID = @id";
                const string query = """
                                    SELECT l.name, l.address
                                    FROM bookings as b
                                    LEFT JOIN lodgings AS l ON b.LodgingID = l.id
                                    WHERE b.LodgingID = @id
                                    """;

                var parameters = new MySqlParameter[]
                {
                    new("@id", ctx.Session.GetInt32("user_id"))
                };

                using var reader = await MySqlHelper.ExecuteReaderAsync(_config.db, query, parameters);
                

                while (await reader.ReadAsync())
                {
                    bookingInfos.Add(new BookingInfo
                    {
                        LodgingName = reader.GetString(0),
                        LodgingAddress = reader.GetString(1)
                    });
                }
            }
        }

        return bookingInfos;
    }

    public async Task CreateBooking(BookingController.Post_Args lodging, HttpContext ctx)
    {
        if(ctx.Session.IsAvailable)
        {
            if(ctx.Session.Keys.Contains("user_id"))
            {
                string query = "INSERT INTO bookings(UserID, LodgingID) VALUES(@UserID, @LodgingID)";
                var parameters = new MySqlParameter[]
                {
                    new("@UserID", ctx.Session.GetInt32("user_id")),
                    new("@LodgingID", lodging.LodgingID),
                };
                await MySqlHelper.ExecuteNonQueryAsync(_config.db, query, parameters);
            }
        }
    }
}