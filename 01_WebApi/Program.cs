using Prometheus;
using WebApi.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.ConfigureControllers();
builder.ConfigureSwagger();
builder.ConfigureDbContext();
builder.ConfigureServices();

var app = builder.Build();

// Run migrations at startup
app.RunMigrationsAtStartup();

app.UseSwagger();
app.UseSwaggerUI();

app.UseMetricServer();
app.UseHttpMetrics();

app.UseAuthorization();
app.MapControllers();

app.Run();
