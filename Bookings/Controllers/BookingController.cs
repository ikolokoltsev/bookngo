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

    [HttpGet]
    [Route("mine")]
    public async Task<IEnumerable<BookingInfo>> dbGetUserBookings() => await _bookingRepository.GetUserBookings(HttpContext);
}