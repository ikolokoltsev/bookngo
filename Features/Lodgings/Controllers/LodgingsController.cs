using server.Features.Lodgings.Models;
using server.Features.Lodgings.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace server.Features.Lodgings.Controllers;

[ApiController]
[Route("lodgings")]
public class LodgingsController(ILodgingRepository _lodgingRepository) : ControllerBase
{
    [HttpGet]
    public async Task<IEnumerable<Lodging>> GetAllLodgings() => await _lodgingRepository.GetAllLodgings();
}