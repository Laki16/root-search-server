using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Cors;
using ApiServer.Models;

namespace ApiServer
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
			services.AddCors(options => {
				options.AddPolicy("DevelopPolicy", builder => {
					// builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
					builder.WithOrigins("https://rootsearch.github.io")
						.WithHeaders("Content-Type")
						.WithMethods("GET", "PUT", "DELETE");
				});
			});

			services.AddControllers();

			// Register the redis cache.
			services.AddStackExchangeRedisCache(options => {
				var redisSettings = Configuration.GetSection("RedisSettings");

				options.Configuration = $"{redisSettings.GetValue<string>("Host")}:{redisSettings.GetValue<string>("Port")}";
				options.InstanceName = redisSettings.GetValue<string>("InstanceName");
			});

			// Register custom cache dependency.
			services.AddScoped<ICacheModule, RedisCacheModule>();

			// Register the search moudle configuration instance.
			services.Configure<SearchEngineApiSettings>(
				Configuration.GetSection("SearchEngineApiSettings")
			);
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

			app.UseCors("DevelopPolicy");

			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
		}
	}
}
