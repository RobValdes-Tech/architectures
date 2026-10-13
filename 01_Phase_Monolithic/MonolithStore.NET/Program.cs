using MonolithStore.NET.Modules.Products;
using MonolithStore.NET.Modules.Orders;

var builder = WebApplication.CreateBuilder(args);

// 1. REGISTRO DE SERVICIOS DEL CONTENEDOR (IoC)
builder.Services.AddControllers();

// Configuración nativa de OpenAPI para la documentación del API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

// Registro de nuestros módulos en memoria para el ejercicio de los Juniors
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IOrderService, OrderService>();

var app = builder.Build();

// 2. CONFIGURACIÓN DEL PIPELINE DE PETICIONES HTTP (Middleware)
if (app.Environment.IsDevelopment())
{
    // Expone el endpoint JSON de OpenAPI en desarrollo (v1.json)
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
