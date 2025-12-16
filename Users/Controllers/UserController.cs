using server.Users.Models;
using server.Users.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.HttpResults;

namespace server.Lodgings.Controllers;

[ApiController]
[Route("users")]
public class UserController(IUserRepository _userRepository) : ControllerBase
{
    public record Post_Args(string Email, string Name, bool Admin, string Password);
    [HttpGet]
    public async Task<IEnumerable<User>> dbGetAllUsers() => await _userRepository.GetAllUsers();


    [HttpPost]
    public async Task dbCreateUser(Post_Args user) => await _userRepository.CreateUser(user);

    [HttpGet]
    [Route("isadmin")]
    public async Task<bool> dbGetAdminStatus() => await _userRepository.GetAdminStatus(HttpContext);
}