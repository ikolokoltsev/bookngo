using Microsoft.AspNetCore.Mvc;
using server;
using server.Travels.Models;
using server.Travels.Repositories;

namespace server.Travels.Controllers;

[ApiController]
[Route("travels")]
public class TravelsController(ITravelRepository _travelRepository) : ControllerBase
{
    public record Post_Args(int TransportID);

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Travel>>> GetAllTravels()
    {
        if (!HttpContext.HasValidSession())
        {
            return Unauthorized("Session missing or expired.");
        }

        try
        {
            var bookings = await _travelRepository.GetAllTravels();
            return Ok(bookings);
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Can't get travels");
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateTravel([FromBody] Post_Args booking)
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
            await _travelRepository.CreateTravel(userId.Value, booking.TransportID);
            return Ok();
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Can't create travel");
        }
    }
}
