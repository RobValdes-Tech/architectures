using Microsoft.EntityFrameworkCore;
using OrderService.Net.Data;
using MassTransit; // <-- Nueva directiva


var builder = WebApplication.CreateBuilder(args);

// 1. REGISTRO DE INFRAESTRUCTURA DE PERSISTENCIA (PostgreSQL)
builder.Services.AddDbContext<OrderDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. CONFIGURACIÓN GLOBAL DE MASSTRANSIT & RABBITMQ
builder.Services.AddMassTransit(x =>
{
    // Indicar que utilizaremos RabbitMQ como broker de transporte
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
