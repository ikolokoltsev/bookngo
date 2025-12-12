using server.Users.Models;
using server.Users.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace server.Lodgings.Controllers;

[ApiController]
[Route("users")]
public class UserController(IUserRepository _userRepository) : ControllerBase
{
    [HttpGet]
    public async Task<IEnumerable<User>> dbGetAllUsers() => await _userRepository.GetAllUsers();
}