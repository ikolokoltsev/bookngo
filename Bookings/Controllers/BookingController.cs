using server.Bookings.Models;
using server.Bookings.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace server.Bookings.Controllers;

[ApiController]
[Route("bookings")]
public class BookingController(IBookingRepository _bookingRepository) : ControllerBase
{
    public record Post_Args(int UserID, int LodgingID);

    [HttpGet]
    public async Task<IEnumerable<Booking>> dbGetAllBookings() => await _bookingRepository.GetAllBookings();
}