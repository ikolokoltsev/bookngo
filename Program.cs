global using MySql.Data.MySqlClient;
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
app.MapGet("/", () => new
{
      status = "running",
      endpoints = new[] {"/login", "/users", "/db"}
      
});
app.MapGet("/profile", Profile.Get);

app.MapDelete("/db", db_reset_to_default);

app.MapPost("/users", Users.Post);



app.MapGet("/me", async (Config config, HttpContext ctx) =>
{
    var user = await Login.Get(config, ctx);

    if (user == null)
      {
            return Results.Unauthorized(); // 401: Not autorized
      }  
    return Results.Ok(new
    {
          Name = user.Name, 
          Email = user.Email,
          Status = "Logged in with cookie!"
    });
}
); 

// app.MapPost("/login", Login.Post);
app.MapPost("/login", async (Login.Post_Args creds, Config config, HttpContext ctx) =>
{
    bool success = await Login.Post(creds, config, ctx);
    return success ? Results.Ok("Logged in!") : Results.Unauthorized();
});

// app.MapGet("/login", Login.Get);
app.MapGet("/login", async (Config config, HttpContext ctx) =>
{
    var user = await Login.Get(config, ctx);
    return user != null ? Results.Ok(user) : Results.Unauthorized();
});
    
app.Run();

async Task db_reset_to_default(Config config)
{
    string query_create_users_table = """
                                      CREATE TABLE users
                                      (
                                        id INT PRIMARY KEY AUTO_INCREMENT,
                                        name VARCHAR(255) NOT NULL,
                                        password VARCHAR(128),
                                        email VARCHAR(255) NOT NULL    
                                      )
                                      """; 
    await MySqlHelper.ExecuteNonQueryAsync(config.db, "DROP TABLE IF EXISTS users");
    await MySqlHelper.ExecuteNonQueryAsync(config.db, query_create_users_table);   
}