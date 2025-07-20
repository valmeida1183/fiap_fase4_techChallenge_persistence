using Consumer;
using Consumer.Extensions;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

builder.Services
    .ConfigureServices()
    .ConfigureDbContext(builder.Configuration)
    .ConfigureMassTransit(builder.Configuration);


var host = builder.Build();
host.Run();
