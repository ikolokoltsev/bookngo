global using MySql.Data.MySqlClient;
using server;
using server.Lodgings.Models;
using server.Lodgings.Repositories;
using System.Text.Json;
using System.Text.Json.Serialization;
using server.Users.Repositories;
using server.Bookings.Repositories;
using server.Transports.Repositories;
using server.Travels.Repositories;
using server.Activities.Repositories;
using server.ActivityBookings.Repositories;

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
builder.Services.AddScoped<ITransportRepository, TransportRepository>();
builder.Services.AddScoped<ITravelRepository, TravelRepository>();
builder.Services.AddScoped<IActivityRepository, ActivityRepository>();
builder.Services.AddScoped<IActivityBookingRepository, ActivityBookingRepository>();

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

app.MapPost("/logout", Login.Logout);

// app.MapPost("/login", Login.Post);
app.MapPost("/login", async (Login.LoginRequest creds, Config config, HttpContext ctx) =>
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

  await MySqlHelper.ExecuteNonQueryAsync(config.db, "DROP TABLE IF EXISTS travels");
  await MySqlHelper.ExecuteNonQueryAsync(config.db, "DROP TABLE IF EXISTS bookings");
  await MySqlHelper.ExecuteNonQueryAsync(config.db, "DROP TABLE IF EXISTS activity_bookings");
  await MySqlHelper.ExecuteNonQueryAsync(config.db, "DROP TABLE IF EXISTS activities");
  await MySqlHelper.ExecuteNonQueryAsync(config.db, "DROP TABLE IF EXISTS transports");
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

  string query_create_transports_table = """
                                          CREATE TABLE transports
                                          (
                                            id INT PRIMARY KEY AUTO_INCREMENT,
                                            name VARCHAR(255) NOT NULL,
                                            origin VARCHAR(100) NOT NULL,
                                            destination VARCHAR(100) NOT NULL,
                                            departure_time DATETIME NOT NULL,
                                            arrival_time DATETIME NOT NULL,
                                            price DOUBLE NOT NULL,
                                            transport_type VARCHAR(50) NOT NULL,
                                            status VARCHAR(50) NOT NULL,
                                            has_wifi BOOL NOT NULL DEFAULT 0,
                                            has_food BOOL NOT NULL DEFAULT 0,
                                            has_premium_class BOOL NOT NULL DEFAULT 0,
                                            description VARCHAR(255)
                                          )
                                          """;
  await MySqlHelper.ExecuteNonQueryAsync(config.db, query_create_transports_table);

  string query_create_activities_table = """
                                          CREATE TABLE activities
                                          (
                                            id INT PRIMARY KEY AUTO_INCREMENT,
                                            name VARCHAR(255) NOT NULL,
                                            country VARCHAR(100) NOT NULL,
                                            city VARCHAR(255) NOT NULL,
                                            address VARCHAR(255) NOT NULL,
                                            start_time DATETIME NOT NULL,
                                            end_time DATETIME NOT NULL,
                                            price DOUBLE NOT NULL,
                                            activity_type VARCHAR(50) NOT NULL,
                                            status VARCHAR(50) NOT NULL,
                                            has_equipment BOOL NOT NULL DEFAULT 0,
                                            has_instructor BOOL NOT NULL DEFAULT 0,
                                            is_indoor BOOL NOT NULL DEFAULT 0,
                                            description VARCHAR(255)
                                          )
                                          """;
  await MySqlHelper.ExecuteNonQueryAsync(config.db, query_create_activities_table);

  string query_create_activity_bookings_table = """
                                                 CREATE TABLE activity_bookings
                                                 (
                                                   id INT PRIMARY KEY AUTO_INCREMENT,
                                                   UserID INT NOT NULL,
                                                   ActivityID INT NOT NULL,
                                                   UNIQUE KEY uq_activity_bookings_user_activity (UserID, ActivityID),
                                                   FOREIGN KEY (UserID) REFERENCES users(id),
                                                   FOREIGN KEY (ActivityID) REFERENCES activities(id)
                                                 )
                                                 """;
  await MySqlHelper.ExecuteNonQueryAsync(config.db, query_create_activity_bookings_table);

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

  string query_create_travels_table = """
                                      CREATE TABLE travels
                                      (
                                        id INT PRIMARY KEY AUTO_INCREMENT,
                                        UserID INT NOT NULL,
                                        TransportID INT NOT NULL,
                                        created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                                        updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
                                        UNIQUE KEY uq_travels_user_transport (UserID, TransportID),
                                        FOREIGN KEY (UserID) REFERENCES users(id),
                                        FOREIGN KEY (TransportID) REFERENCES transports(id)
                                      )
                                      """;
  await MySqlHelper.ExecuteNonQueryAsync(config.db, query_create_travels_table);

  string seed_lodgings = """
                          INSERT INTO lodgings (name, price, country, city, address, rating, status, has_wifi, has_parking, has_pool, has_gym) VALUES
                          ('Seaside Escape', 120.00, 'USA', 'Miami', '123 Ocean View', 4.6, 'Available', 1, 1, 1, 0),
                          ('Mountain Cabin', 95.00, 'USA', 'Aspen', '45 Pine Rd', 4.8, 'Booked', 0, 1, 0, 1),
                          ('City Loft', 150.00, 'USA', 'San Francisco', '789 Market St', 4.2, 'Unavailable', 1, 0, 0, 1),
                          ('Lake House Retreat', 180.00, 'USA', 'Lake Tahoe', '12 Lakeside Dr', 4.9, 'UnderMaintenance', 1, 1, 1, 1),
                          ('Downtown Studio', 85.00, 'USA', 'Austin', '210 Center Ave', 4.3, 'PendingApproval', 1, 0, 0, 0)
                          """;
  await MySqlHelper.ExecuteNonQueryAsync(config.db, seed_lodgings);

  string seed_transports = """
                            INSERT INTO transports (name, origin, destination, departure_time, arrival_time, price, transport_type, status, has_wifi, has_food, has_premium_class, description) VALUES
                            ('Flight AB123', 'Stockholm', 'Berlin', '2025-01-10 09:30:00', '2025-01-10 11:45:00', 199.00, 'Flight', 'Available', 1, 1, 1, 'Morning flight with carry-on included'),
                            ('Train IC42', 'Gothenburg', 'Copenhagen', '2025-01-12 07:15:00', '2025-01-12 10:05:00', 79.50, 'Train', 'Available', 1, 0, 0, 'Direct route with WiFi'),
                            ('Bus 88', 'Oslo', 'Bergen', '2025-01-15 13:00:00', '2025-01-15 20:30:00', 45.00, 'Bus', 'Delayed', 0, 0, 0, 'Weather delays expected')
                            """;
  await MySqlHelper.ExecuteNonQueryAsync(config.db, seed_transports);

  string seed_activities = """
                            INSERT INTO activities (name, country, city, address, start_time, end_time, price, activity_type, status, has_equipment, has_instructor, is_indoor, description) VALUES
                            ('Scuba Dive Intro', 'Spain', 'Mallorca', '12 Harbor Pier', '2025-02-10 09:00:00', '2025-02-10 12:00:00', 120.00, 'Diving', 'Available', 1, 1, 0, 'Beginner-friendly dive with equipment included'),
                            ('Surf Lesson', 'Portugal', 'Porto', '45 Atlantic Ave', '2025-03-05 14:00:00', '2025-03-05 16:00:00', 75.00, 'Surfing', 'Available', 1, 1, 0, 'Small group lesson with boards included'),
                            ('Ski Day Pass', 'Sweden', 'Are', 'Ski Center 3', '2025-01-20 08:00:00', '2025-01-20 17:00:00', 95.00, 'Skiing', 'SoldOut', 0, 0, 0, 'Lift access for the full day')
                            """;
  await MySqlHelper.ExecuteNonQueryAsync(config.db, seed_activities);

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

  string seed_travels = """
                                    INSERT INTO travels (UserID, TransportID) VALUES
                                    (1, 1)
                                    """;
  await MySqlHelper.ExecuteNonQueryAsync(config.db, seed_travels);

  string seed_activity_bookings = """
                                  INSERT INTO activity_bookings (UserID, ActivityID) VALUES
                                  (1, 1)
                                  """;
  await MySqlHelper.ExecuteNonQueryAsync(config.db, seed_activity_bookings);
}
