using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Pinball.Api.Data;
using Pinball.Api.Services.Interfaces;
using Pinball.Api.Services.Interfaces.Impl;
using Pinball.OpdbClient.Entities;
using Pinball.OpdbClient.Interfaces;

namespace Pinball.Api
{
    public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddHttpClient();

			services.AddDbContext<PinballDbContext>(options =>
				options.UseSqlServer(Configuration.GetConnectionString("PinballDbContext")));

			services.Configure<OpdbClientOptions>(Configuration.GetSection("Opdb"));
			services.AddScoped<IOpdbClient, OpdbClient.Interfaces.Impl.OpdbClient>();
			services.AddScoped<IPinballMachineCatalogService, PinballMachineCatalogService>();
			services.AddScoped<ITestOpdbService, TestOpdbService>();

			services.AddControllers();

            services.AddMvcCore().AddApiExplorer();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "PinIQ API", Version = "v1" });
            });
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseHttpsRedirection();

			app.UseRouting();

			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "PinIQ API V1");
            });

			app.UseStaticFiles();
		}
	}
}
