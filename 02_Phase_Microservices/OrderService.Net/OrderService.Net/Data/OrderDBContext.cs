using Microsoft.EntityFrameworkCore;
using OrderService.Net.Models;

namespace OrderService.Net.Data
{
    public class OrderDbContext : DbContext
    {
        public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options)
        {
        }

        // Set de datos expuesto para interactuar con la tabla de órdenes
        public DbSet<Order> Orders => Set<Order>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Garantizar la generación correcta del esquema relacional
            modelBuilder.Entity<Order>().HasKey(o => o.Id);
        }
    }
}
