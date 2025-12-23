namespace server;

static class Login
{
    public record Get_Data(string? Name, string Email);
    public static async Task<Get_Data?> Get(Config config, HttpContext ctx)
    {
        Get_Data? result = null;

        if(ctx.Session.IsAvailable)
        {
            if(ctx.Session.Keys.Contains("user_id"))
            {
                string query = "SELECT name, email FROM users WHERE id = @id";
                var parameters = new MySqlParameter[]
                {
                    new("@id", ctx.Session.GetInt32("user_id"))
                };

                using(var reader = await MySqlHelper.ExecuteReaderAsync(config.db, query, parameters))
                {
                    if(reader.Read())
                    {
                        if(reader[0] is string name)
                        {
                            result = new(name, reader.GetString(1));
                        }
                    }
                }
            }
        }
        return result;
    }

    public record Post_Args(string Email, string Password);
   
    static public void Logout(HttpContext ctx)
    {
        ctx.Session.Clear();
    }


 public static async Task<bool> Post(Post_Args credentials, Config config, HttpContext ctx)
{
    const int MAX_ATTEMPTS = 5;
    bool result = false;

    string query = """
        SELECT id, password, failed_attempts, locked_until
        FROM users
        WHERE email = @email
    """;

    using var reader = await MySqlHelper.ExecuteReaderAsync(
        config.db,
        query,
        new MySqlParameter[] { new("@email", credentials.Email) }
    );

    if (!reader.Read())
        return false;

    int userId = reader.GetInt32("id");
    string dbPassword = reader.GetString("password");
    int failedAttempts = reader.GetInt32("failed_attempts");
    DateTime? lockedUntil = reader.IsDBNull(3)
        ? null
        : reader.GetDateTime("locked_until");

    if (lockedUntil != null && lockedUntil > DateTime.UtcNow)
        return false;

    if (dbPassword != credentials.Password)
    {
        failedAttempts++;

        if (failedAttempts >= MAX_ATTEMPTS)
        {
            await MySqlHelper.ExecuteNonQueryAsync(
                config.db,
                """
                UPDATE users
                SET failed_attempts = @fails,
                    locked_until = DATE_ADD(UTC_TIMESTAMP(), INTERVAL 5 MINUTE)
                WHERE id = @id
                """,
                new MySqlParameter[]
                {
                    new("@fails", failedAttempts),
                    new("@id", userId)
                }
            );
        }
        else
        {
            await MySqlHelper.ExecuteNonQueryAsync(
                config.db,
                """
                UPDATE users
                SET failed_attempts = @fails
                WHERE id = @id
                """,
                new MySqlParameter[]
                {
                    new("@fails", failedAttempts),
                    new("@id", userId)
                }
            );
        }

        return false;
    }

    await MySqlHelper.ExecuteNonQueryAsync(
        config.db,
        """
        UPDATE users
        SET failed_attempts = 0,
            locked_until = NULL
        WHERE id = @id
        """,
        new MySqlParameter[] { new("@id", userId) }
    );

    ctx.Session.SetInt32("user_id", userId);
    return true;
}   
};
