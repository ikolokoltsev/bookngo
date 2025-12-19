using server.Orders.Models;

namespace server.Orders.Repositories;

public interface IOrderRepository
{
    Task<IEnumerable<OrderSummary>> GetUserOrders(int userId);
    Task<OrderDetail?> GetOrderById(int userId, int orderId);
    Task<OrderCreateResult?> CreateOrder(int userId, OrderCreateRequest request);
}
