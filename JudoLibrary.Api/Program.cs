using System.Collections.Generic;
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
                
                // If Environment is Development seed Combines data
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
                    
                    context.Add(new SubCategory
                    {
                        Id = "sutemi-waza", Name = "Sutemi waza", Description = "Sacrifice techniques", CategoryId = "nage-waza"
                    });
                    
                    context.Add(new SubCategory
                    {
                        Id = "osaekomi-waza", Name = "Osaekomi waza ", Description = "Pins or matholds", CategoryId = "katame-waza"
                    });
                    
                    // Seeding Techniques
                    context.Add(new Technique
                    {
                        Id = "kouchi-gari", Name = "Kouchi gari", Description = "Small inner reap", Category = "nage-waza", SubCategory = "ashi-waza"
                    });
                    
                    context.Add(new Technique
                    {
                        Id = "osoto-gari", Name = "Osoto gari ", Description = "Major or large outer reap", Category = "nage-waza", SubCategory = "ashi-waza"
                    });
                    
                    context.Add(new Technique
                    {
                        Id = "ushiro-goshi", Name = "Ushiro goshi", Description = "Rear hip throw", Category = "nage-waza", SubCategory = "koshi-waza"
                    });
                    
                    context.Add(new Technique
                    {
                        Id = "tani-otoshi", Name = "Tani otoshi", Description = "Valley drop", Category = "nage-waza", SubCategory = "sutemi-waza"
                    });
                    
                    context.Add(new Technique
                    {
                        Id = "Kesa-gatame", Name = "Kesa-gatame", Description = "Scarf hold", Category = "katame-waza", SubCategory = "osaekomi-waza"
                    });
                    
                    // * Main Technique *
                    context.Add(new Technique
                    {
                        Id = "seoi-nage", Name = "Seoi Nage", Description = "Shoulder throw", Category = "nage-waza", SubCategory = "te-waza",
                        SetUpAttacks = new List<TechniqueSetupAttack>
                        {
                            new TechniqueSetupAttack{TechniqueId = "seoi-nage", SetUpAttackId = "kouchi-gari"}
                        },
                        FollowUpAttacks = new List<TechniqueFollowupAttack>
                        {
                            new TechniqueFollowupAttack{TechniqueId = "seoi-nage", FollowUpAttackId = "osoto-gari"}
                        },
                        Counters = new List<TechniqueCounter>
                        {
                            new TechniqueCounter{TechniqueId = "seoi-nage", CounterId = "ushiro-goshi"}, 
                            new TechniqueCounter{TechniqueId = "seoi-nage", CounterId = "tani-otoshi"}
                        }
                    });
                    
                    // Seeding submissions
                    context.Add(new Submission
                    {
                        TechniqueId = "seoi-nage",
                        Video = "seoi1.mp4",
                        Description = "This Seoi nage was very hard to pull of..."
                    });
                    
                    context.Add(new Submission
                    {
                        TechniqueId = "osoto-gari",
                        Video = "osoto1.mp4",
                        Description = "Demonstration of Osoto Gari"
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