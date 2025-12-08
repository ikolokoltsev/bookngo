global using MySql.Data.MySqlClient;
using Microsoft.Extensions.Options;
using server;

var builder = WebApplication.CreateBuilder(args);

// Start: Session koniguration, "in memory cache"
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
      options.Cookie.HttpOnly = true;
      options.Cookie.IsEssential = true;
});
// End: Session koniguration, "in memory cache"

Config config = new("server=127.0.0.1;uid=bookngo;pwd=bookngo;database=bookngo;");
builder.Services.AddSingleton(config);
var app = builder.Build();
app.UseSession();

app.MapPost("/users", Users.Post);
app.MapDelete("/db", db_reset_to_default);
    
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



