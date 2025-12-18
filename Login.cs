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
    public static async Task<bool> Post(Post_Args credentials, Config config, HttpContext ctx)
    {
        bool result = false;
        string query = "SELECT id FROM users WHERE email = @email AND password = @password";
        var parameters = new MySqlParameter[]
        {
            new("@email", credentials.Email),
            new("@password", credentials.Password),
        };

        object query_result = await MySqlHelper.ExecuteScalarAsync(config.db, query, parameters);

        if(query_result is int id)
        {
            ctx.Session.SetInt32("user_id", id);
            result = true;
        }

        return result;
    }
    static public void Logout(HttpContext ctx)
    {
        ctx.Session.Clear();
    }
}
