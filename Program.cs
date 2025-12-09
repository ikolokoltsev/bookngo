global using Npgsql;
using server;

var builder = WebApplication.CreateBuilder(args);

Config config = new("server=127.0.0.1;uid=bookngo;pwd=pass;database=bookngo;");
builder.Services.AddSingleton(config);
var app = builder.Build();

app.MapPost("/users", Users.Post);

app.MapDelete("/db", db_reset_to_default);

app.Run();

async Task db_reset_to_default(Config config)
{
    string query_create_users_table = """
        CREATE TABLE users
        (
            id SERIAL PRIMARY KEY,
            name VARCHAR(255),
            email VARCHAR(254) NOT NULL UNIQUE,
            password VARCHAR(128)
        )
    """;

    using var conn = new NpgsqlConnection(config.db);
    await conn.OpenAsync();

    using var cmdDrop = new NpgsqlCommand("DROP TABLE IF EXISTS users", conn);
    await cmdDrop.ExecuteNonQueryAsync();

    using var cmdCreate = new NpgsqlCommand(query_create_users_table, conn);
    await cmdCreate.ExecuteNonQueryAsync();
}
