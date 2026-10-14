using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MassTransit; // <-- Nueva directiva
using OrderService.Net.Data;
using OrderService.Net.Models;
using OrderService.Net.Shared.Events; // <-- Contrato inmutable del evento

namespace OrderService.Net.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrdersController : ControllerBase
    {
        private readonly OrderDbContext _context;
        private readonly IPublishEndpoint _publishEndpoint; // <-- Bus de datos inyectado

        public OrdersController(OrderDbContext context, IPublishEndpoint publishEndpoint)
        {
            _context = context;
            _publishEndpoint = publishEndpoint;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            return await _context.Orders.ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<Order>> CreateOrder([FromBody] OrderCreateDto dto)
        {
            var newOrder = new Order
            {
                Id = Guid.NewGuid(),
                UserId = dto.UserId,
                ProductId = dto.ProductId,
                Quantity = dto.Quantity,
                CreatedAt = DateTime.UtcNow
            };

            // 1. Persistencia Relacional en PostgreSQL
            _context.Orders.Add(newOrder);
            await _context.SaveChangesAsync();

            // 2. Publicación Asíncrona en el Broker de RabbitMQ
            await _publishEndpoint.Publish(new OrderCreatedEvent
            {
                OrderId = newOrder.Id,
                UserId = newOrder.UserId,
                ProductId = newOrder.ProductId,
                Quantity = newOrder.Quantity,
                CreatedAt = newOrder.CreatedAt
            });

            return CreatedAtAction(nameof(GetOrders), new { id = newOrder.Id }, newOrder);
        }
    }

    public class OrderCreateDto
    {
        public string UserId { get; set; } = string.Empty;
        public string ProductId { get; set; } = string.Empty;
        public int Quantity { get; set; }
    }
}
