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

        const string query = "SELECT Id, Name, Password, Admin, Email FROM users";
        using var reader = await MySqlHelper.ExecuteReaderAsync(_config.db, query);

        while (await reader.ReadAsync())
        {
            users.Add(new User
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Password = reader.GetString(2),
                Admin = reader.GetBoolean(3),
                Email = reader.GetString(4),
            });
        }

        return users;
    }
}