using Microsoft.AspNetCore.Mvc;
using server.Users.Models;
using System.Diagnostics;
using server.Lodgings.Controllers;

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

    public async Task CreateUser(UserController.Post_Args user, HttpContext ctx)
    {
        bool IsAdmin = false;
        IsAdmin = await GetAdminStatus(ctx);
        if(IsAdmin)
        {
        string query = "INSERT INTO users(name, email, admin, password) VALUES(@name, @email, @admin, @password)";
        var parameters = new MySqlParameter[]
        {
            new("@name", user.Name),
            new("@email", user.Email),
            new("@admin", user.Admin),
            new("@password", user.Password)
        };
        await MySqlHelper.ExecuteNonQueryAsync(_config.db, query, parameters);
        }
    }

    public async Task<bool> GetAdminStatus(HttpContext ctx)
    {
        Debug.Assert(ctx != null);
        var user = await Login.Get(_config, ctx);

        if(ctx.Session.IsAvailable)
        {
            if(ctx.Session.Keys.Contains("user_id"))
            {
                string query = "SELECT Admin FROM users WHERE id = @id";
                var parameters = new MySqlParameter[]
                {
                    new("@id", ctx.Session.GetInt32("user_id"))
                };

                using(var reader = await MySqlHelper.ExecuteReaderAsync(_config.db, query, parameters))
                {
                    if(reader.Read())
                    {
                        if(reader[0] is bool admin)
                        {
                            return admin;
                        }
                    }
                }
            }
        }
        return false;
    }
}