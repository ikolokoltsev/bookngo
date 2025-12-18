using server.Lodgings.Models;
using server.Lodgings.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace server.Lodgings.Controllers;

[ApiController]
[Route("lodgings")]
public class LodgingsController(ILodgingRepository _lodgingRepository) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<LodgingData>>> GetLodgings([FromQuery] LodgingFilterQuery filter)
    {
        try
        {
            var lodgings = await _lodgingRepository.GetAllLodgings(filter);
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
        try
        {
            await _lodgingRepository.CreateLodging(lodging, HttpContext);
            return Ok();
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Can't create lodging");
        }
    }
}
