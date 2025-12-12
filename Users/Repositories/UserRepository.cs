using server.Users.Models;

namespace server.Users.Repositories;

public class UserRepository : IUserRepository
{
    private readonly Config _config;

    public UserRepository(Config config)
    {
        _config = config;
    }

    public async Task<IEnumerable<User>> GetAllUsers()
    {
        var users = new List<User>();

        return users;
    }
}