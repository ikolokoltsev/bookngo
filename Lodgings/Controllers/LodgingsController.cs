using server.Lodgings.Models;
using server.Lodgings.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace server.Lodgings.Controllers;

[ApiController]
[Route("lodging")]
public class LodgingsController(ILodgingRepository _lodgingRepository) : ControllerBase
{
    [HttpGet]
    public async Task<IEnumerable<Lodging>> GetAllLodgings() => await _lodgingRepository.GetAllLodgings();

    // [HttpGet("filter")]
    // public async Task<IEnumerable<Lodging>> GetFilteredLodgings([FromQuery] LodgingFilterQuery filter)
    // {
    //     return await _lodgingRepository.GetFilteredLodgings(filter);
    // }

    [HttpGet("filter")]
    public async Task<IEnumerable<Lodging>> GetFilteredLodgings([FromQuery]LodgingFilterQuery filter)
    {
        return await _lodgingRepository.GetFilteredLodgings(filter);
    } 
}
