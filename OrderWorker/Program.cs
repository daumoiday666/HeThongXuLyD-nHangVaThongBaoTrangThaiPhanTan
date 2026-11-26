using Microsoft.EntityFrameworkCore;
using OrderWorker;
using OrderWorker.Data;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHostedService<OrderConsumer>();

var host = builder.Build();
host.Run();
