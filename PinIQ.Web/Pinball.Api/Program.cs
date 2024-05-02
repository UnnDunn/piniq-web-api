using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Identity;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Pinball.Api.Data;
using Pinball.Api.Entities.Configuration;
using Pinball.Api.Services.Entities.Configuration;
using Pinball.Api.Services.Interfaces;
using Pinball.Api.Services.Interfaces.Impl;
using Pinball.Entities.Api.Responses;
using Pinball.Entities.Opdb;
using Pinball.OpdbClient.Interfaces;
using Serilog;
using Serilog.Sinks.ApplicationInsights.TelemetryConverters;
using AppleAuthenticationOptions = Pinball.Api.Entities.Configuration.AppleAuthenticationOptions;

namespace Pinball.Api;

public partial class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var loggerConfiguration = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration);

        // Azure KeyVault configuration
        var azKeyVaultConnectionString = builder.Configuration.GetConnectionString("AzKeyVault");

        if (!string.IsNullOrEmpty(azKeyVaultConnectionString))
            // found a key vault connection string, so configure azure key-vault
            builder.Configuration.AddAzureKeyVault(new Uri(azKeyVaultConnectionString), new DefaultAzureCredential());

        // ApplicationInsights options
        var appInsightsOptions = builder.Configuration.GetSection("Logging:ApplicationInsights")
            .Get<ApplicationInsightsOptions>();
        var appInsightsConnectionString = builder.Configuration.GetConnectionString("ApplicationInsights");
        if ((appInsightsOptions?.IsEnabled ?? false) && !string.IsNullOrEmpty(appInsightsConnectionString))
        {
            // configure application insights here
            var telemetryConfig = new TelemetryConfiguration { ConnectionString = appInsightsConnectionString };
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
        builder.Services.Configure<DeveloperOptions>(builder.Configuration.GetSection("DeveloperOptions"));
        builder.Services.AddScoped<IOpdbClient, OpdbClient.Interfaces.Impl.OpdbClient>();
        builder.Services.AddScoped<IPinballMachineCatalogService, PinballMachineCatalogService>();
        builder.Services.AddScoped<ITestOpdbService, TestOpdbService>();
        builder.Services.AddScoped<LoginService>();

        // Authentication
        builder.Services.Configure<MyJwtBearerOptions>(
            builder.Configuration.GetSection("Authentication:Schemes:Bearer"));
        var myJwtBearerOptions =
            builder.Configuration.GetRequiredSection("Authentication:Schemes:Bearer").Get<MyJwtBearerOptions>();
        var authBuilder = builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            {
                options.ForwardDefaultSelector = ctx => ctx.Request.Path.StartsWithSegments("/api")
                    ? JwtBearerDefaults.AuthenticationScheme
                    : null;
            })
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, o =>
            {
                if (myJwtBearerOptions is null) throw new Exception("Cannot read Jwt Bearer configuration");

                var validIssuerSigningKey =
                    myJwtBearerOptions.SigningKeys.First(s => s.Issuer == myJwtBearerOptions.ValidIssuer);
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = myJwtBearerOptions.ValidIssuer,
                    ValidAudiences = myJwtBearerOptions.ValidAudiences,
                    IssuerSigningKey =
                        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(validIssuerSigningKey.Value))
                };
            });

        var appleAuthOptions = builder.Configuration.GetSection("Authentication:MyOptions:Apple")
            .Get<AppleAuthenticationOptions>();
        if (appleAuthOptions is not null)
        {
            authBuilder.AddApple(options =>
            {
                options.ClientId = appleAuthOptions.ClientId;
                options.TeamId = appleAuthOptions.TeamId;
                options.KeyId = appleAuthOptions.KeyId;
                if (!string.IsNullOrEmpty(appleAuthOptions.PrivateKey))
                {
                    Log.Logger.Debug("Setting up Apple Sign-in with options {options}", appleAuthOptions);
                    options.GenerateClientSecret = true;
                    options.PrivateKey = (_, _) => Task.FromResult(appleAuthOptions.PrivateKey.AsMemory());
                }
                else
                {
                    var pkFile =
                        builder.Environment.ContentRootFileProvider.GetFileInfo(
                            $"AuthKey_{appleAuthOptions.KeyId}.p8");
                    if (pkFile.Exists)
                    {
                        options.UsePrivateKey(_ => pkFile);
                    }
                    else
                    {
                        Log.Logger.Error("Could not find private key for Apple Sign-in Provider");
                        throw new Exception("Could not find private key for Apple Sign-in provider");
                    }
                }
            });

            var googleAuthOptions = builder.Configuration.GetSection("Authentication:MyOptions:Google")
                .Get<OauthAuthenticationOptions>();
            if (googleAuthOptions is not null)
                authBuilder.AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
                {
                    options.ClientId = googleAuthOptions.ClientId;
                    options.ClientSecret = googleAuthOptions.ClientSecret;
                });
        }

        builder.Services.AddControllers()
            .AddJsonOptions(static options =>
                options.JsonSerializerOptions.TypeInfoResolverChain.Add(PiniqJsonSerializerContext.Default));

        builder.Services.AddMvcCore().AddApiExplorer();

        builder.Services.AddAuthorization(o =>
        {
            o.FallbackPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
        });

        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "PinIQ API", Version = "v1" });
        });

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
            app.UseDeveloperExceptionPage();
        else
            app.UseExceptionHandler("/error");

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseAuthorization();

        app.MapControllers();

        app.UseSwagger();

        app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "PinIQ API V1"); });

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