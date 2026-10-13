using Microsoft.AspNetCore.Mvc;
using MonolithStore.NET.Modules.Products;
using MonolithStore.NET.Modules.Orders;

namespace MonolithStore.NET.Controllers
{
    [ApiController]
    [Route("api/store")]
    public class StoreController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IOrderService _orderService;

        // Inyección de dependencias a través del constructor unificado
        public StoreController(IProductService productService, IOrderService orderService)
        {
            _productService = productService;
            _orderService = orderService;
        }

        [HttpPost("checkout")]
        public IActionResult CompleteCheckout([FromBody] CheckoutRequest request)
        {
            // 1. Validar el Stock usando el módulo de productos en memoria
            var hasStock = _productService.VerifyStock(request.ProductId, request.Quantity);
            if (!hasStock)
            {
                return BadRequest(new { Message = "No hay suficiente stock del producto solicitado." });
            }

            // 2. Crear la orden usando el módulo de órdenes transaccionales
            var orderId = _orderService.CreateOrder(request.UserId, request.ProductId, request.Quantity);

            return Ok(new { 
                Message = "Transacción completada con éxito en el Monolito", 
                OrderId = orderId,
                Status = "Completado"
            });
        }
    }

    // DTO para mapear la petición del cliente de forma segura
    public class CheckoutRequest
    {
        public string UserId { get; set; } = string.Empty;
        public string ProductId { get; set; } = string.Empty;
        public int Quantity { get; set; }
    }
}
