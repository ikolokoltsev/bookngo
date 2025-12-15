using server.Users.Models;

namespace server.Users.Repositories;

public interface IUserRepository
{
    Task<IEnumerable<User>> GetAllUsers();
}