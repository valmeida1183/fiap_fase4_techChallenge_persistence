using Application.Service;
using Application.Service.Interface;
using Consumer.Consumers;
using Core.Repository.Interface;
using Infraestructure.Configuration;
using Infraestructure.Repository;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Consumer.Extensions;
public static class ConsumerConfigurationExtension
{
    public static IServiceCollection ConfigureMassTransit(this IServiceCollection services, IConfiguration configuration)
    {
        var host = configuration.GetSection("MassTransit:Host").Value;
        var user = configuration.GetSection("MassTransit:User").Value;
        var password = configuration.GetSection("MassTransit:Password").Value;

        if (string.IsNullOrEmpty(host) || string.IsNullOrEmpty(user) || string.IsNullOrEmpty(password))
            throw new Exception("Missing environment variables to configure MassTransit");

        services.AddMassTransit(x =>
        {
            x.AddConsumer<CreateContactConsumer>();

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(host, "/", h =>
                {
                    h.Username(user);
                    h.Password(password);
                });

                cfg.ConfigureEndpoints(context);
            });
        });

        return services;
    }

    public static IServiceCollection ConfigureDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<DataContext>(options =>
        {
            options.UseSqlServer(connectionString);
        });

        return services;
    }

    public static IServiceCollection ConfigureServices(this IServiceCollection services)
    {
        services.AddScoped<IContactService, ContactService>();
        services.AddScoped<IDirectDistanceDialingService, DirectDistanceDialingService>();

        services.AddScoped<IContactRepository, ContactRepository>();
        services.AddScoped<IDirectDistanceDialingRepository, DirectDistanceDialingRepository>();

        return services;
    }
}
