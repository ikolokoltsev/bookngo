global using MySql.Data.MySqlClient;
using server;
using server.Lodgings.Models;
using server.Lodgings.Repositories;
using System.Text.Json;
using System.Text.Json.Serialization;
using server.Users.Repositories;
using server.Bookings.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Start: Session koniguration, "in memory cache"
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
  options.Cookie.HttpOnly = true;
  options.Cookie.IsEssential = true;
});
// End: Session koniguration, "in memory cache"

builder.Services.AddControllers().AddJsonOptions(options =>
{
  options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
});
builder.Services.AddScoped<ILodgingRepository, LodgingRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();

var dbPort = Environment.GetEnvironmentVariable("DB_PORT");
Config config = new($"server=127.0.0.1;port={dbPort};uid=bookngo;pwd=bookngo;database=bookngo;");
builder.Services.AddSingleton(config);
var app = builder.Build();

app.UseSession();
app.MapGet("/", () => new
{
  status = "running",
  endpoints = new[] { "/login", "/users", "/db", }

});
app.MapGet("/profile", Profile.Get);

app.MapDelete("/db", db_reset_to_default);

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

app.MapControllers();
app.Run();

async Task db_reset_to_default(Config config)
{

  await MySqlHelper.ExecuteNonQueryAsync(config.db, "DROP TABLE IF EXISTS bookings");
  await MySqlHelper.ExecuteNonQueryAsync(config.db, "DROP TABLE IF EXISTS users");
  await MySqlHelper.ExecuteNonQueryAsync(config.db, "DROP TABLE IF EXISTS lodgings");

  string query_create_users_table = """
                                      CREATE TABLE users
                                      (
                                        id INT PRIMARY KEY AUTO_INCREMENT,
                                        name VARCHAR(255) NOT NULL,
                                        password VARCHAR(128),
                                        admin BOOL,
                                        email VARCHAR(255) NOT NULL    
                                      )
                                      """;
  await MySqlHelper.ExecuteNonQueryAsync(config.db, query_create_users_table);

  string query_create_lodgings_table = """
                                        CREATE TABLE lodgings
                                        (
                                          id INT PRIMARY KEY AUTO_INCREMENT,
                                          name VARCHAR(255) NOT NULL,
                                          country VARCHAR(100) NOT NULL,
                                          city VARCHAR(255) NOT NULL,
                                          price DOUBLE NOT NULL,
                                          rating DOUBLE NOT NULL,
                                          address VARCHAR(255) NOT NULL,
                                          status VARCHAR(50) NOT NULL,
                                          has_wifi BOOL NOT NULL DEFAULT 0,
                                          has_parking BOOL NOT NULL DEFAULT 0,
                                          has_pool BOOL NOT NULL DEFAULT 0,
                                          has_gym BOOL NOT NULL DEFAULT 0,
                                          description VARCHAR(255)
                                        )
                                        """;
  await MySqlHelper.ExecuteNonQueryAsync(config.db, query_create_lodgings_table);

  string query_create_bookings_table = """
                                      CREATE TABLE bookings
                                      (
                                        UserID INT NOT NULL,
                                        LodgingID INT NOT NULL,
                                        PRIMARY KEY (UserID, LodgingID),
                                        FOREIGN KEY (UserID) REFERENCES users(id),
                                        FOREIGN KEY (LodgingID) REFERENCES lodgings(id) 
                                      )
                                      """;
  await MySqlHelper.ExecuteNonQueryAsync(config.db, query_create_bookings_table);

  string seed_lodgings = """
                          INSERT INTO lodgings (name, price, country, city, address, rating, status, has_wifi, has_parking, has_pool, has_gym) VALUES
                          ('Seaside Escape', 120.00, 'USA', 'Miami', '123 Ocean View', 4.6, 'Available', 1, 1, 1, 0),
                          ('Mountain Cabin', 95.00, 'USA', 'Aspen', '45 Pine Rd', 4.8, 'Booked', 0, 1, 0, 1),
                          ('City Loft', 150.00, 'USA', 'San Francisco', '789 Market St', 4.2, 'Unavailable', 1, 0, 0, 1),
                          ('Lake House Retreat', 180.00, 'USA', 'Lake Tahoe', '12 Lakeside Dr', 4.9, 'UnderMaintenance', 1, 1, 1, 1),
                          ('Downtown Studio', 85.00, 'USA', 'Austin', '210 Center Ave', 4.3, 'PendingApproval', 1, 0, 0, 0)
                          """;
  await MySqlHelper.ExecuteNonQueryAsync(config.db, seed_lodgings);

  string seed_users = """
                          INSERT INTO users (name, email, admin, password) VALUES
                          ('Oscar', 'email', true, 's3cret')
                          """;
  await MySqlHelper.ExecuteNonQueryAsync(config.db, seed_users);

  string seed_bookings = """
                          INSERT INTO bookings (UserID, LodgingID) VALUES
                          (1, 1)
                          """;
  await MySqlHelper.ExecuteNonQueryAsync(config.db, seed_bookings);
}
