using JudoLibrary.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace JudoLibrary.Api
{
    public class Startup
    {
        private const string AllCors = "All";
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // Adds support for using controllers
            services.AddControllers();
            
            // Opening Session with Postgres SQL
            // services.AddDbContext<AppDbContext>(options => 
            //     options.UseNpgsql(_configuration.GetConnectionString("JudoLibrary")));
            
            // Adding service for using EF In-Memory database
            services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("Dev"));

            // Service for CORS mechanism, with current policy, everything is accepted
            services.AddCors(options => options
                .AddPolicy(AllCors,build => build
                    .AllowAnyHeader()
                    .AllowAnyOrigin()
                    .AllowAnyMethod()));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Middleware func for CORS mechanism
            app.UseCors(AllCors);
            
            // Support for routing
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}