using Microsoft.EntityFrameworkCore;
using SuperComData.Context;
using SuperComWorker;

var builder = Host.CreateApplicationBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddWindowsService();
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
await host.RunAsync();
