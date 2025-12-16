using server.Lodgings.Models;
using server.Lodgings.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace server.Lodgings.Controllers;

[ApiController]
[Route("lodgings")]
public class LodgingsController(ILodgingRepository _lodgingRepository) : ControllerBase
{
    [HttpGet]
    public async Task<IEnumerable<LodgingData>> GetLodgings([FromQuery] LodgingFilterQuery filter) => await _lodgingRepository.GetAllLodgings(filter);

    [HttpGet]
    [Route("{id}")]
    public async Task<LodgingDetail?> GetLodgingById(int id) => await _lodgingRepository.GetLodgingById(id);

    [HttpPost]
    public async Task PostLodging([FromBody] Lodging lodging) => await _lodgingRepository.CreateLodging(lodging);
}
