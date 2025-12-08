global using MySql.Data.MySqlClient;
using server;

var builder = WebApplication.CreateBuilder(args);

// Start: Session koniguration, "in memory cache"

// End: Session koniguration, "in memory cache"

Config config = new("server=127.0.0.1;uid=bookngo;pwd=bookngo;database=bookngo;");
builder.Services.AddSingleton(config);
var app = builder.Build();

app.MapDelete("/db", db_reset_to_default);

app.MapPost("/users", async (Config config, string Name, string Email, string Password) =>
{
    string sql = "INSERT INTO users(name, email, password) VALUES (@name, @email, @password)";

    using var conn = new MySqlConnection(config.db);
    await conn.OpenAsync();

    using var cmd = new MySqlCommand(sql, conn);
    cmd.Parameters.AddWithValue("@name", Name);
    cmd.Parameters.AddWithValue("@email", Email);
    cmd.Parameters.AddWithValue("@password", Password);

    await cmd.ExecuteNonQueryAsync();

    return Results.Ok("User created!");
});
app.Run();

async Task db_reset_to_default(Config config)
{
    string query_create_users_table = """
                                      CREATE TABLE users
                                      (
                                        id INT PRIMARY KEY AUTO_INCREMENT,
                                        name VARCHAR(255) NOT NULL,
                                        password VARCHAR(255) NOT NULL,
                                        email VARCHAR(255) NOT NULL    
                                      )
                                      """; 
    await MySqlHelper.ExecuteNonQueryAsync(config.db, query_create_users_table);
}



