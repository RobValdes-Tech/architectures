using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderService.Net.Data;
using OrderService.Net.Models;

namespace OrderService.Net.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrdersController : ControllerBase
    {
        private readonly OrderDbContext _context;

        // Inyección de dependencias nativa del DbContext configurado
        public OrdersController(OrderDbContext context)
        {
            _context = context;
        }

        // Endpoint HTTP GET para listar todas las órdenes registradas en PostgreSQL
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            return await _context.Orders.ToListAsync();
        }

        // Endpoint HTTP POST para registrar una nueva orden de compra de forma transaccional
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

            // Guardar el registro en la tabla de PostgreSQL de forma asíncrona
            _context.Orders.Add(newOrder);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetOrders), new { id = newOrder.Id }, newOrder);
        }
    }

    // DTO seguro para tipar los datos recibidos del cliente externo
    public class OrderCreateDto
    {
        public string UserId { get; set; } = string.Empty;
        public string ProductId { get; set; } = string.Empty;
        public int Quantity { get; set; }
    }
}
