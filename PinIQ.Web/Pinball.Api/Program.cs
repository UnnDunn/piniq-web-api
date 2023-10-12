using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Pinball.Api.Data;

namespace Pinball.Api
{
    public partial class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            CreateDbIfNotExists(host).Wait();
           
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        private static async Task UpdateDatabaseAsync(IHost host)
        {
            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;

            try
            {
                var context = services.GetRequiredService<PinballDbContext>();
                await context.Database.MigrateAsync();
            } catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "A problem occurred updating the database schema");
            }
        }

        private static async Task CreateDbIfNotExists(IHost host)
        {
            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;
            var logger = services.GetRequiredService<ILogger<Program>>();
            LogProcessCreatingDatabase(logger);

            try
            {
                var context = services.GetRequiredService<PinballDbContext>();
                await context.Database.EnsureCreatedAsync();
            }
            catch (Exception ex)
            {
                LogErrorUpdatingDatabase(logger, ex);
            }
        }

        [LoggerMessage(EventId = 1101, Level = LogLevel.Information, Message = "Creating database")]
        private static partial void LogProcessCreatingDatabase(ILogger<Program> logger);

        [LoggerMessage(EventId = 1102, Level = LogLevel.Error, Message = "A problem occurred updating the database")]
        private static partial void LogErrorUpdatingDatabase(ILogger<Program> logger, Exception ex);
    }
}
