using System;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Pinball.Api.Data;
using Pinball.Api.Entities.Configuration;
using Pinball.Api.Services.Interfaces;
using Pinball.Api.Services.Interfaces.Impl;
using Pinball.OpdbClient.Entities;
using Pinball.OpdbClient.Interfaces;
using Serilog;
using Serilog.Sinks.ApplicationInsights.TelemetryConverters;

namespace Pinball.Api;

public partial class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var loggerConfiguration = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration);
        
        // ApplicationInsights options
        var appInsightsOptions = builder.Configuration.GetSection("Logging:ApplicationInsights")
            .Get<ApplicationInsightsOptions>();
        var appInsightsConnectionString = builder.Configuration.GetConnectionString("ApplicationInsights");
        if ((appInsightsOptions?.IsEnabled ?? false) && !string.IsNullOrEmpty(appInsightsConnectionString))
        {
            // configure application insights here
            var telemetryConfig = new TelemetryConfiguration()
                { ConnectionString = appInsightsConnectionString };
            loggerConfiguration.WriteTo.ApplicationInsights(telemetryConfig, new TraceTelemetryConverter());
        }
        
        Log.Logger = loggerConfiguration.CreateLogger();
        builder.Host.UseSerilog();

        builder.Services.AddHttpClient();
			
        builder.Services.AddDbContext<PinballDbContext>(options =>
        {
            options.UseSqlServer(builder.Configuration.GetConnectionString("PinballDbContext"));
        });
			
			
        builder.Services.Configure<OpdbClientOptions>(builder.Configuration.GetSection("Opdb"));
        builder.Services.AddScoped<IOpdbClient, OpdbClient.Interfaces.Impl.OpdbClient>();
        builder.Services.AddScoped<IPinballMachineCatalogService, PinballMachineCatalogService>();
        builder.Services.AddScoped<ITestOpdbService, TestOpdbService>();

        builder.Services.AddControllers();

        builder.Services.AddMvcCore().AddApiExplorer();

        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "PinIQ API", Version = "v1" });
        });
        
        var app = builder.Build();
        
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseAuthorization();

        app.MapControllers();

        app.UseSwagger();

        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "PinIQ API V1");
        });

        app.UseStaticFiles();
        
        
        CreateDbIfNotExists(app).Wait();
           
        app.Run();
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