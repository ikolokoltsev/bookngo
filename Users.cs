namespace server;

static class Users
{
    public record Post_Args(string Name, string Email, string Password);
    public static async Task Post(Post_Args user, Config config)
    {
        string query = "INSERT INTO users(name, email, password) VALUES(@name, @email, @password)";

        using var conn = new NpgsqlConnection(config.db);
        await conn.OpenAsync();

        using var cmd = new NpgsqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@name", user.Name);
        cmd.Parameters.AddWithValue("@email", user.Email);
        cmd.Parameters.AddWithValue("@password", user.Password);

        await cmd.ExecuteNonQueryAsync();
    }
}