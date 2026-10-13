namespace MonolithStore.NET.Modules.Orders
{
    public interface IOrderService
    {
        Guid CreateOrder(string userId, string productId, int quantity);
    }

    public class OrderService : IOrderService
    {
        public Guid CreateOrder(string userId, string productId, int quantity)
        {
            // Genera un identificador único en memoria simulando el insert en base de datos
            return Guid.NewGuid();
        }
    }
}
