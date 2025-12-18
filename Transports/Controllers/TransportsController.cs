using Microsoft.AspNetCore.Mvc;
using server;
using server.Transports.Models;
using server.Transports.Repositories;

namespace server.Transports.Controllers;

[ApiController]
[Route("transports")]
public class TransportsController(ITransportRepository _transportRepository, Config _config) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TransportData>>> GetTransports([FromQuery] TransportFilterQuery filter)
    {
        if (!HttpContext.HasValidSession())
        {
            return Unauthorized("Session missing or expired.");
        }

        try
        {
            var transports = await _transportRepository.GetAllTransports(filter);
            return Ok(transports);
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Can't get transports");
        }
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<ActionResult<TransportDetail?>> GetTransportById(int id)
    {
        if (!HttpContext.HasValidSession())
        {
            return Unauthorized("Session missing or expired.");
        }

        try
        {
            var transport = await _transportRepository.GetTransportById(id);
            return Ok(transport);
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Can't get transport");
        }
    }

    [HttpPost]
    public async Task<IActionResult> PostTransport([FromBody] Transport transport)
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
            await _transportRepository.CreateTransport(transport);
            return Ok();
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Can't create transport");
        }
    }

    [HttpDelete]
    [Route("{id}")]
    public async Task<IActionResult> DeleteTransport(int id)
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
            await _transportRepository.DeleteTransport(id);
            return Ok();
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Can't delete transport");
        }
    }
}
