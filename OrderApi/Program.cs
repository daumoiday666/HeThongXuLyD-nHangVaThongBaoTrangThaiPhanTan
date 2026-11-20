using Microsoft.EntityFrameworkCore;
using OrderApi.Data;
using OrderApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/orders", async (AppDbContext dbContext) =>
{
    var orders = await dbContext.Orders.ToListAsync();
    return orders;
})
.WithName("GetOrders");

app.MapPost("/orders", async (CreateOrderRequest request, AppDbContext dbContext) =>
{
    var order = new Order
    {
        Id = Guid.NewGuid(),
        CustomerName = request.CustomerName,
        ProductName = request.ProductName,
        Quantity = request.Quantity,
        Status = "Pending",
        CreatedAt = DateTime.UtcNow
    };

    dbContext.Orders.Add(order);
    await dbContext.SaveChangesAsync();

    return Results.Created($"/orders/{order.Id}", order);
})
.WithName("CreateOrder");

app.Run();
