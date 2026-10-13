namespace MonolithStore.NET.Modules.Products
{
    public interface IProductService
    {
        bool VerifyStock(string productId, int quantity);
    }

    public class ProductService : IProductService
    {
        public bool VerifyStock(string productId, int quantity)
        {
            // Simulación pedagógica para Juniors: asumimos que siempre hay stock para cualquier ID excepto vacíos
            return !string.IsNullOrWhiteSpace(productId) && quantity > 0;
        }
    }
}
