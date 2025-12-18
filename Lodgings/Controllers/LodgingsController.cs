using server.Lodgings.Models;
using server.Lodgings.Repositories;
using Microsoft.AspNetCore.Mvc;
using server;

namespace server.Lodgings.Controllers;

[ApiController]
[Route("lodgings")]
public class LodgingsController(ILodgingRepository _lodgingRepository, Config _config) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<LodgingData>>> GetLodgings([FromQuery] LodgingFilterQuery filter)
    {
        if (!HttpContext.HasValidSession())
        {
            return Unauthorized("Session missing or expired.");
        }

        try
        {
            IEnumerable<LodgingData> lodgings = await _lodgingRepository.GetAllLodgings(filter);
            return Ok(lodgings);
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Can't get lodgings");
        }
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<ActionResult<LodgingDetail?>> GetLodgingById(int id)
    {
        if (!HttpContext.HasValidSession())
        {
            return Unauthorized("Session missing or expired.");
        }

        try
        {
            var lodging = await _lodgingRepository.GetLodgingById(id);
            return Ok(lodging);
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Can't get lodging");
        }
    }

    [HttpPost]
    public async Task<IActionResult> PostLodging([FromBody] Lodging lodging)
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
            await _lodgingRepository.CreateLodging(lodging);
            return Ok();
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Can't create lodging");
        }
    }

    [HttpDelete]
    [Route("{id}")]
    public async Task<IActionResult> DeleteLodging(int id)
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
            await _lodgingRepository.DeleteLodging(id);
            return Ok();
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Can't delete lodging");
        }
    }
}
