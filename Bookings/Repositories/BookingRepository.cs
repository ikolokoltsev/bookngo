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

        foreach(Booking booking in bookings)
        {
            System.Console.WriteLine(booking.UserID);
        }

        return bookings;
    }

    // public async Task<Booking?> GetUserBookings(int id)
    // {
        
    // }

    // public async Task CreateBookings(BookingController.Post_Args booking, HttpContext ctx)
    // {
        
    // }
}