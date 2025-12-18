using Microsoft.AspNetCore.Mvc;
using server;
using server.Activities.Models;
using server.Activities.Repositories;

namespace server.Activities.Controllers;

[ApiController]
[Route("activities")]
public class ActivitiesController(IActivityRepository _activityRepository, Config _config) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ActivityData>>> GetActivities([FromQuery] ActivityFilterQuery filter)
    {
        if (!HttpContext.HasValidSession())
        {
            return Unauthorized("Session missing or expired.");
        }

        try
        {
            var activities = await _activityRepository.GetAllActivities(filter);
            return Ok(activities);
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Can't get activities");
        }
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<ActionResult<ActivityDetail?>> GetActivityById(int id)
    {
        if (!HttpContext.HasValidSession())
        {
            return Unauthorized("Session missing or expired.");
        }

        try
        {
            var activity = await _activityRepository.GetActivityById(id);
            return Ok(activity);
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Can't get activity");
        }
    }

    [HttpPost]
    public async Task<IActionResult> PostActivity([FromBody] Activity activity)
    {
        if (!HttpContext.HasValidSession())
        {
            return Unauthorized("Session missing or expired.");
        }

        if (!await HttpContext.IsAdminAsync(_config))
        {
            return Forbid("Admin access required.");
        }

        try
        {
            await _activityRepository.CreateActivity(activity);
            return Ok();
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Can't create activity");
        }
    }

    [HttpPatch]
    [Route("{id}")]
    public async Task<IActionResult> UpdateActivity(int id, [FromBody] ActivityUpdateRequest update)
    {
        if (!HttpContext.HasValidSession())
        {
            return Unauthorized("Session missing or expired.");
        }

        if (!await HttpContext.IsAdminAsync(_config))
        {
            return Forbid("Admin access required.");
        }

        if (update == null || !update.HasUpdates())
        {
            return BadRequest("No fields to update.");
        }

        try
        {
            var updated = await _activityRepository.UpdateActivity(id, update);
            return updated ? Ok() : NotFound();
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Can't update activity");
        }
    }

    [HttpDelete]
    [Route("{id}")]
    public async Task<IActionResult> DeleteActivity(int id)
    {
        if (!HttpContext.HasValidSession())
        {
            return Unauthorized("Session missing or expired.");
        }

        if (!await HttpContext.IsAdminAsync(_config))
        {
            return Forbid("Admin access required.");
        }

        try
        {
            await _activityRepository.DeleteActivity(id);
            return Ok();
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Can't delete activity");
        }
    }
}
