using server.Bookings.Models;
using server.Bookings.Controllers;

namespace server.Bookings.Repositories;

public interface IBookingRepository
{
    Task<IEnumerable<Booking>> GetAllBookings();
    Task<IEnumerable<BookingInfo>> GetUserBookings(HttpContext ctx);
    // Task CreateBookings(BookingController.Post_Args booking, HttpContext ctx);
}
