using System.Reflection;
using DAL;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Service;
using Service.Interface;

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .Build();

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddMassTransit(x =>
        {
            var assembly = Assembly.GetEntryAssembly();
            var setting =  configuration.GetSection("Bus").Get<MassTransitOption>();
            if (setting is null)
            {
                throw new ArgumentNullException($"{nameof(MassTransitOption)} setting is null");
            }

            x.AddConsumers(assembly);

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host($"rabbitmq://{setting.Url}/{setting.Host}", h =>
                {
                    h.Username(setting.User);
                    h.Password(setting.Password);
                });

                cfg.ConfigureEndpoints(context, KebabCaseEndpointNameFormatter.Instance);
                cfg.UseJsonSerializer();
            });
        });
        
        services.AddDbContext<ApplicationDbContext>();
        services.AddLogging(configure: builder => builder.SetMinimumLevel(LogLevel.Debug));
        services.AddScoped<IDeliveryService, DeliveryService>();
    })
    .Build();
    
await host.RunAsync();