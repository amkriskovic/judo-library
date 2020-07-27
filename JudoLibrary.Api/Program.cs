using JudoLibrary.Data;
using JudoLibrary.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace JudoLibrary.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Bundle
            var host = CreateHostBuilder(args).Build();
            
            // Extract the services before running server
            using (var scope = host.Services.CreateScope())
            {
                // Extracting context for AppDbContext and Hosting Environment
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var environment = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();
                
                // If Environment is Development seed test data
                if (environment.IsDevelopment())
                {
                    // Seeding Categories
                    context.Add(new Category {Id = "nage-waza", Name = "Nage Waza", Description = "Throwing Techniques"});
                    context.Add(new Category {Id = "katame-waza", Name = "Katame Waza", Description = "Grappling Techniques"});
                    context.Add(new Category {Id = "atemi-waza", Name = "Atemi Waza", Description = "Body Striking Techniques"});
                    context.Add(new Category {Id = "uke-waza", Name = "Uke Waza", Description = "Blocks And Parries"});
                    
                    // Seeding SubCategories
                    context.Add(new SubCategory
                    {
                        Id = "te-waza", Name = "Te Waza", Description = "Hand throwing techniques", CategoryId = "nage-waza"
                    });
                    context.Add(new SubCategory
                    {
                        Id = "koshi-waza", Name = "Koshi Waza", Description = "Hip throwing techniques", CategoryId = "nage-waza"
                    });
                    context.Add(new SubCategory
                    {
                        Id = "ashi-waza", Name = "Ashi Waza", Description = "Foot throwing techniques", CategoryId = "nage-waza"
                    });
                    
                    // Seeding Techniques
                    context.Add(new Technique
                    {
                        Id = "seoi-nage", Name = "Seoi Nage", Description = "Shoulder throw", SubCategoryId = "te-waza"
                    });

                    // Saving changes to in-memory DB
                    context.SaveChanges();
                }
            }
            
            // Launch App
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
}