using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PrismaProject.AsyncDataServices;
using PrismaProject.Models;
using PrismaProject.Services;
using PrismaProject.Data;
using PrismaProject.EventProcessing;
using Serilog;

namespace PrismaProject;

class Program
{
    static async Task Main(string[] args)
    {
        //setup main Serilog logger using config 
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Console()
            .WriteTo.File("../logs/platform-service.log", rollingInterval: RollingInterval.Day)
            .CreateLogger();

       

        //working with configuration
        IConfiguration _config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", true, true)
            .Build();
        
        //create worker , RabbitMQ listener (it should be a microservice )
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureLogging((context, builder) => builder.AddSerilog(dispose: true))
            .ConfigureServices(services =>
            {
                services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
                services.AddHostedService<MessageBusSubscriber>();
                services.Configure<MessageStoreDbSettings>(_config.GetSection("MessageStoreDatabase"));
                services.AddTransient<IMessageRepo, MessageRepo>();
                services.AddTransient<IEventProcessor, EventProcessor>();
            })
            .Build(); // Build the host, as per configurations.
        
        //create new collection of service handlers            
        IServiceCollection services = new ServiceCollection();
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        services.AddLogging(logbuilder => logbuilder.AddSerilog(dispose: true));
        services.Configure<MessageStoreDbSettings>(_config.GetSection("MessageStoreDatabase"));
        services.AddTransient<IProducerService, Producer>();
        services.AddTransient<IMessageRepo, MessageRepo>();
        services.AddTransient<IEventProcessor, EventProcessor>();
        services.AddSingleton<IMessageBusClient, MessageBusClient>();
        services.AddSingleton<IConfiguration>(_config);
        services.AddSingleton<App>();

        
        await host.StartAsync();

        var builder = services.BuildServiceProvider();
        var app = builder.GetService<App>();
        if (app != null)
        {
            await app.SendMessage("hello 1");
            await app.GetMessages();
        }

        await Task.Delay(TimeSpan.FromSeconds(1));
        Log.Information("Done");
    }
}