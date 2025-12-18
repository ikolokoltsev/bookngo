using server.Users.Models;
using server.Users.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace server.Lodgings.Controllers;

[ApiController]
[Route("users")]
public class UserController(IUserRepository _userRepository) : ControllerBase
{
    public record Post_Args(string Email, string Name, bool Admin, string Password);
    [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> dbGetAllUsers()
    {
        try
        {
            var users = await _userRepository.GetAllUsers();
            return Ok(users);
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Can't get users");
        }
    }


    [HttpPost]
    public async Task<IActionResult> dbCreateUser(Post_Args user)
    {
        try
        {
            await _userRepository.CreateUser(user, HttpContext);
            return Ok();
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Can't get user");
        }
    }

    [HttpGet]
    [Route("isadmin")]
    public async Task<ActionResult<bool>> dbGetAdminStatus()
    {
        try
        {
            var isAdmin = await _userRepository.GetAdminStatus(HttpContext);
            return Ok(isAdmin);
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Can't get admin status");
        }
    }
}
