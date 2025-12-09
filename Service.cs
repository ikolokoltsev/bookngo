namespace server; 
static class Service
{
      public record Post_Args(int Id, string Name, string Category);

      public static async Task Post(Post_Args user, Config config)
      {
            string query = "INSERT INTO service(id, name, category) VALUES(@id, @name, @category)";
            var parameters = new MySqlParameter[]
            {
            new("@name", user.Id),
            new("@email", user.Name),
            new("@password", user.Category)
            };

            await MySqlHelper.ExecuteNonQueryAsync(config.db, query, parameters);
      }
    public record Get_Data(string name, string? Name);
    public static async Task<Get_Data?>
    Get(Config config, HttpContext ctx)
    {
        Get_Data? result = null;

        if(ctx.Session.GetInt32("user_id") is int user_id)
        {
            string query = "SELECT id, name, category FROM service";
            var parameters = new MySqlParameter[] {new("@id", user_id)};
            using (var reader = await MySqlHelper.ExecuteReaderAsync(config.db, query, parameters))
                {
                    if(reader.Read())
                    {
                        result = new(reader.GetString(0), reader[1] as string);
                    }
                }
        }
        return result; 
    }   
}


