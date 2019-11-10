using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SecretSanta.EmailSender;
using System;
using System.IO;

namespace SecretSanta
{
  class Program
  {
    static void Main(string[] args)
    {

      // Create service collection and configure our services
      var services = ConfigureServices();
      // Generate a provider
      var serviceProvider = services.BuildServiceProvider();

      // Kick off our actual code
      serviceProvider.GetService<SecretSantaApplication>().Run();

      //var builder = new ConfigurationBuilder()
      //  .SetBasePath(Directory.GetCurrentDirectory())
      //  .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

      //IConfigurationRoot configuration = builder.Build();

      //Console.WriteLine(configuration.GetConnectionString("Storage"));

    }

    private static IServiceCollection ConfigureServices()
    {
      IServiceCollection services = new ServiceCollection();

      // Set up the objects we need to get to configuration settings
      var config = LoadConfiguration();

      var appConfig = new AppConfig();

      config.Bind(appConfig);

      
      // Add the config to our DI container for later user
      services.AddSingleton(appConfig);

      services.AddSingleton(typeof(IEmailSender), typeof(EmailSender.EmailSender));

      services.AddTransient<SecretSantaApplication>();
      return services;
    }

    public static IConfiguration LoadConfiguration()
    {
      var builder = new ConfigurationBuilder()
          .SetBasePath(Directory.GetCurrentDirectory())
          .AddJsonFile("appsettings.json", optional: true,
                       reloadOnChange: true);
      return builder.Build();
    }
  }
}
