using server.Bookings.Models;
using server.Bookings.Controllers;

namespace server.Bookings.Repositories;

public interface IBookingRepository
{
    Task<IEnumerable<Booking>> GetAllBookings();
    // Task<Booking?> GetUserBookings(int id);
    // Task CreateBookings(BookingController.Post_Args booking, HttpContext ctx);
}
