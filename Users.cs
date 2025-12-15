namespace server;

static class Users_old
{
      public record Post_Args(string Name, string Email, bool Admin, string Password);

      public static async Task Post(Post_Args user, Config config)
      {
            string query = "INSERT INTO users(name, email, admin, password) VALUES(@name, @email, @admin, @password)";
            var parameters = new MySqlParameter[]
            {
            new("@name", user.Name),
            new("@email", user.Email),
            new("@admin", user.Admin),
            new("@password", user.Password)
            };

            await MySqlHelper.ExecuteNonQueryAsync(config.db, query, parameters);
      }
}

// string sql = "INSERT INTO users(name, email, password) VALUES (@name, @email, @password)";

//     using var conn = new MySqlConnection(config.db);
//     await conn.OpenAsync();

//     using var cmd = new MySqlCommand(sql, conn);
//     cmd.Parameters.AddWithValue("@name", Name);
//     cmd.Parameters.AddWithValue("@email", Email);
//     cmd.Parameters.AddWithValue("@password", Password);

//     await cmd.ExecuteNonQueryAsync();

//     return Results.Ok("User created!");
// });