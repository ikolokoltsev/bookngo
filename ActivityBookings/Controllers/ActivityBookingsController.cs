using Microsoft.AspNetCore.Mvc;
using server;
using server.ActivityBookings.Models;
using server.ActivityBookings.Repositories;

namespace server.ActivityBookings.Controllers;

[ApiController]
[Route("activity-bookings")]
public class ActivityBookingsController(IActivityBookingRepository _activityBookingRepository) : ControllerBase
{
    public record ActivityBookingRequest(int ActivityID);

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ActivityBooking>>> GetAllActivityBookings()
    {
        if (!HttpContext.HasValidSession())
        {
            return Unauthorized("Session missing or expired.");
        }

        try
        {
            var bookings = await _activityBookingRepository.GetAllActivityBookings();
            return Ok(bookings);
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Can't get activity bookings");
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateActivityBooking([FromBody] ActivityBookingRequest activityBooking)
    {
        if (!HttpContext.HasValidSession())
        {
            return Unauthorized("Session missing or expired.");
        }

        var userId = HttpContext.Session.GetInt32("user_id");
        if (!userId.HasValue)
        {
            return Unauthorized("Session missing or expired.");
        }

        try
        {
            await _activityBookingRepository.CreateActivityBooking(userId.Value, activityBooking.ActivityID);
            return Ok();
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Can't create activity booking");
        }
    }
}
