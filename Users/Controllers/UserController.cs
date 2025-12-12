using server.Lodgings.Models;
using server.Lodgings.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace server.Lodgings.Controllers;

[ApiController]
[Route("lodging")]
public class UserController(ILodgingRepository _userRepository) : ControllerBase
{
    [HttpGet]
    public async Task<IEnumerable<Lodging>> GetAllLodgings() => await _userRepository.GetAllLodgings();
}