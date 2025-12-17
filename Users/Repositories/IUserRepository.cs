using Microsoft.AspNetCore.Mvc;
using server.Users.Models;
using server.Lodgings.Controllers;

namespace server.Users.Repositories;

public interface IUserRepository
{
    Task<IEnumerable<User>> GetAllUsers();
    Task CreateUser(UserController.Post_Args user, HttpContext ctx);
    Task<bool> GetAdminStatus(HttpContext ctx);
}