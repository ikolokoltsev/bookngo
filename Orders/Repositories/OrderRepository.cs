using server.Orders.Models;

namespace server.Orders.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly Config _config;

    public OrderRepository(Config config)
    {
        _config = config;
    }

    public async Task<IEnumerable<OrderSummary>> GetUserOrders(int userId)
    {
        var orders = new List<OrderSummary>();
        const string query = "SELECT Id, UserID, total_amount, created_at FROM orders WHERE UserID = @userId";
        var parameters = new MySqlParameter[] { new("@userId", userId) };

        using var reader = await MySqlHelper.ExecuteReaderAsync(_config.db, query, parameters);
        while (await reader.ReadAsync())
        {
            orders.Add(new OrderSummary
            {
                Id = reader.GetInt32(0),
                UserID = reader.GetInt32(1),
                TotalAmount = reader.GetDouble(2),
                CreatedAt = reader.GetDateTime(3)
            });
        }

        return orders;
    }

    public async Task<OrderDetail?> GetOrderById(int userId, int orderId)
    {
        const string orderQuery = "SELECT Id, UserID, total_amount, created_at FROM orders WHERE Id = @id AND UserID = @userId";
        var parameters = new MySqlParameter[]
        {
            new("@id", orderId),
            new("@userId", userId)
        };

        using var orderReader = await MySqlHelper.ExecuteReaderAsync(_config.db, orderQuery, parameters);
        if (!await orderReader.ReadAsync())
        {
            return null;
        }

        var detail = new OrderDetail
        {
            Id = orderReader.GetInt32(0),
            UserID = orderReader.GetInt32(1),
            TotalAmount = orderReader.GetDouble(2),
            CreatedAt = orderReader.GetDateTime(3)
        };

        detail.Lodgings = await GetOrderLodgings(orderId);
        detail.Travels = await GetOrderTravels(orderId);
        detail.Activities = await GetOrderActivities(orderId);

        return detail;
    }

    public async Task<OrderCreateResult?> CreateOrder(int userId, OrderCreateRequest request)
    {
        if (request.LodgingIDs == null || request.LodgingIDs.Count == 0)
        {
            return null;
        }

        var lodgingIds = request.LodgingIDs;
        var travelIds = request.TravelIDs ?? new List<int>();
        var activityBookingIds = request.ActivityBookingIDs ?? new List<int>();

        var lodgingItems = await GetLodgingItems(userId, lodgingIds);
        if (lodgingItems.Count == 0)
        {
            return null;
        }

        var travelItems = travelIds.Count > 0 ? await GetTravelItems(userId, travelIds) : new List<OrderTravelItem>();
        var activityItems = activityBookingIds.Count > 0 ? await GetActivityItems(userId, activityBookingIds) : new List<OrderActivityItem>();

        double totalAmount = lodgingItems.Sum(item => item.Price)
                             + travelItems.Sum(item => item.Price)
                             + activityItems.Sum(item => item.Price);

        using var connection = new MySqlConnection(_config.db);
        await connection.OpenAsync();

        using var insertOrder = new MySqlCommand("INSERT INTO orders (UserID, total_amount) VALUES (@userId, @totalAmount)", connection);
        insertOrder.Parameters.AddWithValue("@userId", userId);
        insertOrder.Parameters.AddWithValue("@totalAmount", totalAmount);
        await insertOrder.ExecuteNonQueryAsync();

        long orderId = insertOrder.LastInsertedId;
        if (orderId == 0)
        {
            using var idCommand = new MySqlCommand("SELECT LAST_INSERT_ID()", connection);
            orderId = (long)(await idCommand.ExecuteScalarAsync() ?? 0);
        }

        if (lodgingIds.Count > 0)
        {
            var lodgingParams = new List<MySqlParameter>
            {
                new("@orderId", orderId),
                new("@userId", userId)
            };
            string lodgingsQueryUpdate = GenerateServiceUpdateQuery("lodgingId", lodgingIds, lodgingParams);
            using var updateLodgings = new MySqlCommand($"""
                UPDATE bookings
                SET OrderID = @orderId
                WHERE UserID = @userId AND OrderID IS NULL AND LodgingID IN ({lodgingsQueryUpdate})
                """, connection);
            updateLodgings.Parameters.AddRange(lodgingParams.ToArray());
            await updateLodgings.ExecuteNonQueryAsync();
        }

        if (travelIds.Count > 0)
        {
            var travelParams = new List<MySqlParameter>
            {
                new("@orderId", orderId),
                new("@userId", userId)
            };
            string travelsQueryUpdate = GenerateServiceUpdateQuery("travelId", travelIds, travelParams);
            using var updateTravels = new MySqlCommand($"""
                UPDATE travels
                SET OrderID = @orderId
                WHERE UserID = @userId AND OrderID IS NULL AND Id IN ({travelsQueryUpdate})
                """, connection);
            updateTravels.Parameters.AddRange(travelParams.ToArray());
            await updateTravels.ExecuteNonQueryAsync();
        }

        if (activityBookingIds.Count > 0)
        {
            var activityParams = new List<MySqlParameter>
            {
                new("@orderId", orderId),
                new("@userId", userId)
            };
            string activityBookingQueryUpdate = GenerateServiceUpdateQuery("activityBookingId", activityBookingIds, activityParams);
            using var updateActivities = new MySqlCommand($"""
                UPDATE activity_bookings
                SET OrderID = @orderId
                WHERE UserID = @userId AND OrderID IS NULL AND Id IN ({activityBookingQueryUpdate})
                """, connection);
            updateActivities.Parameters.AddRange(activityParams.ToArray());
            await updateActivities.ExecuteNonQueryAsync();
        }

        return new OrderCreateResult((int)orderId, totalAmount);
    }

    private async Task<List<OrderLodgingItem>> GetOrderLodgings(int orderId)
    {
        var items = new List<OrderLodgingItem>();
        const string query = """
                             SELECT l.Id, l.Name, l.Address, l.Price
                             FROM bookings b
                             INNER JOIN lodgings l ON b.LodgingID = l.Id
                             WHERE b.OrderID = @orderId
                             """;
        var parameters = new MySqlParameter[] { new("@orderId", orderId) };

        using var reader = await MySqlHelper.ExecuteReaderAsync(_config.db, query, parameters);
        while (await reader.ReadAsync())
        {
            items.Add(new OrderLodgingItem
            {
                LodgingID = reader.GetInt32(0),
                Name = reader.GetString(1),
                Address = reader.GetString(2),
                Price = reader.GetDouble(3)
            });
        }

        return items;
    }

    private async Task<List<OrderTravelItem>> GetOrderTravels(int orderId)
    {
        var items = new List<OrderTravelItem>();
        const string query = """
                             SELECT t.Id, tr.Id, tr.Name, tr.departure_time, tr.arrival_time, tr.Price
                             FROM travels t
                             INNER JOIN transports tr ON t.TransportID = tr.Id
                             WHERE t.OrderID = @orderId
                             """;
        var parameters = new MySqlParameter[] { new("@orderId", orderId) };

        using var reader = await MySqlHelper.ExecuteReaderAsync(_config.db, query, parameters);
        while (await reader.ReadAsync())
        {
            items.Add(new OrderTravelItem
            {
                TravelID = reader.GetInt32(0),
                TransportID = reader.GetInt32(1),
                Name = reader.GetString(2),
                DepartureTime = reader.GetDateTime(3),
                ArrivalTime = reader.GetDateTime(4),
                Price = reader.GetDouble(5)
            });
        }

        return items;
    }

    private async Task<List<OrderActivityItem>> GetOrderActivities(int orderId)
    {
        var items = new List<OrderActivityItem>();
        const string query = """
                             SELECT ab.Id, a.Id, a.Name, a.start_time, a.end_time, a.Price
                             FROM activity_bookings ab
                             INNER JOIN activities a ON ab.ActivityID = a.Id
                             WHERE ab.OrderID = @orderId
                             """;
        var parameters = new MySqlParameter[] { new("@orderId", orderId) };

        using var reader = await MySqlHelper.ExecuteReaderAsync(_config.db, query, parameters);
        while (await reader.ReadAsync())
        {
            items.Add(new OrderActivityItem
            {
                ActivityBookingID = reader.GetInt32(0),
                ActivityID = reader.GetInt32(1),
                Name = reader.GetString(2),
                StartTime = reader.GetDateTime(3),
                EndTime = reader.GetDateTime(4),
                Price = reader.GetDouble(5)
            });
        }

        return items;
    }

    private async Task<List<OrderLodgingItem>> GetLodgingItems(int userId, List<int> lodgingIds)
    {
        var items = new List<OrderLodgingItem>();
        if (lodgingIds.Count == 0)
        {
            return items;
        }

        var ids = string.Join(",", lodgingIds);

        string query = $"""
                        SELECT l.Id, l.Name, l.Address, l.Price
                        FROM bookings b
                        INNER JOIN lodgings l ON b.LodgingID = l.Id
                        WHERE b.UserID = @userId AND b.OrderID IS NULL AND b.LodgingID IN ({ids})
                        """;

        var parameters = new MySqlParameter[] { new("@userId", userId) };
        using var reader = await MySqlHelper.ExecuteReaderAsync(_config.db, query, parameters);
        while (await reader.ReadAsync())
        {
            items.Add(new OrderLodgingItem
            {
                LodgingID = reader.GetInt32(0),
                Name = reader.GetString(1),
                Address = reader.GetString(2),
                Price = reader.GetDouble(3)
            });
        }

        return items;
    }

    private async Task<List<OrderTravelItem>> GetTravelItems(int userId, List<int> travelIds)
    {
        var items = new List<OrderTravelItem>();
        if (travelIds.Count == 0)
        {
            return items;
        }

        var ids = string.Join(",", travelIds);

        string query = $"""
                        SELECT t.Id, tr.Id, tr.Name, tr.departure_time, tr.arrival_time, tr.Price
                        FROM travels t
                        INNER JOIN transports tr ON t.TransportID = tr.Id
                        WHERE t.UserID = @userId AND t.OrderID IS NULL AND t.Id IN ({ids})
                        """;

        var parameters = new MySqlParameter[] { new("@userId", userId) };
        using var reader = await MySqlHelper.ExecuteReaderAsync(_config.db, query, parameters);
        while (await reader.ReadAsync())
        {
            items.Add(new OrderTravelItem
            {
                TravelID = reader.GetInt32(0),
                TransportID = reader.GetInt32(1),
                Name = reader.GetString(2),
                DepartureTime = reader.GetDateTime(3),
                ArrivalTime = reader.GetDateTime(4),
                Price = reader.GetDouble(5)
            });
        }

        return items;
    }

    private async Task<List<OrderActivityItem>> GetActivityItems(int userId, List<int> activityBookingIds)
    {
        var items = new List<OrderActivityItem>();
        if (activityBookingIds.Count == 0)
        {
            return items;
        }

        var ids = string.Join(",", activityBookingIds);

        string query = $"""
                        SELECT ab.Id, a.Id, a.Name, a.start_time, a.end_time, a.Price
                        FROM activity_bookings ab
                        INNER JOIN activities a ON ab.ActivityID = a.Id
                        WHERE ab.UserID = @userId AND ab.OrderID IS NULL AND ab.Id IN ({ids})
                        """;

        var parameters = new MySqlParameter[] { new("@userId", userId) };
        using var reader = await MySqlHelper.ExecuteReaderAsync(_config.db, query, parameters);
        while (await reader.ReadAsync())
        {
            items.Add(new OrderActivityItem
            {
                ActivityBookingID = reader.GetInt32(0),
                ActivityID = reader.GetInt32(1),
                Name = reader.GetString(2),
                StartTime = reader.GetDateTime(3),
                EndTime = reader.GetDateTime(4),
                Price = reader.GetDouble(5)
            });
        }

        return items;
    }

    private static string GenerateServiceUpdateQuery(string prefix, List<int> ids, List<MySqlParameter> parameters)
    {
        var placeholders = new List<string>();
        for (int i = 0; i < ids.Count; i++)
        {
            string name = $"@{prefix}{i}";
            placeholders.Add(name);
            parameters.Add(new MySqlParameter(name, ids[i]));
        }

        return string.Join(", ", placeholders);
    }
}
