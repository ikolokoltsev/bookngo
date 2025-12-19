using Microsoft.AspNetCore.Mvc;
using server;
using server.Orders.Models;
using server.Orders.Repositories;

namespace server.Orders.Controllers;

[ApiController]
[Route("orders")]
public class OrdersController(IOrderRepository _orderRepository) : ControllerBase
{
    [HttpGet]
    [Route("mine")]
    public async Task<ActionResult<IEnumerable<OrderSummary>>> GetMyOrders()
    {
        if (!HttpContext.HasValidSession())
        {
            return Unauthorized("Session missing or expired.");
        }

        try
        {
            var userId = HttpContext.Session.GetInt32("user_id");
            if (!userId.HasValue)
            {
                return Unauthorized("Session missing or expired.");
            }

            var orders = await _orderRepository.GetUserOrders(userId.Value);
            return Ok(orders);
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Can't get orders");
        }
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<ActionResult<OrderDetail?>> GetOrderById(int id)
    {
        if (!HttpContext.HasValidSession())
        {
            return Unauthorized("Session missing or expired.");
        }

        try
        {
            var userId = HttpContext.Session.GetInt32("user_id");
            if (!userId.HasValue)
            {
                return Unauthorized("Session missing or expired.");
            }

            var order = await _orderRepository.GetOrderById(userId.Value, id);
            return order == null ? NotFound() : Ok(order);
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Can't get order");
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] OrderCreateRequest request)
    {
        if (!HttpContext.HasValidSession())
        {
            return Unauthorized("Session missing or expired.");
        }

        if (request == null || request.LodgingIDs == null || request.LodgingIDs.Count == 0)
        {
            return BadRequest("Order must include at least one booking.");
        }

        try
        {
            var userId = HttpContext.Session.GetInt32("user_id");
            if (!userId.HasValue)
            {
                return Unauthorized("Session missing or expired.");
            }

            var result = await _orderRepository.CreateOrder(userId.Value, request);
            if (result == null)
            {
                return BadRequest("Invalid booking, travel, or activity booking IDs.");
            }

            return Ok(result);
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Can't create order");
        }
    }
}
