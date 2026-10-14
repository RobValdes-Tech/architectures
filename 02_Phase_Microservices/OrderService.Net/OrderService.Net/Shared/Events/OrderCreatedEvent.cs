namespace OrderService.Net.Shared.Events
{
    // Record inmutable que representa el evento de negocio
    public record OrderCreatedEvent
    {
        public Guid OrderId { get; init; }
        public string UserId { get; init; } = string.Empty;
        public string ProductId { get; init; } = string.Empty;
        public int Quantity { get; init; }
        public DateTime CreatedAt { get; init; }
    }
}
