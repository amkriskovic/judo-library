using System.Collections.Generic;
using System.Security.Claims;
using JudoLibrary.Data;
using JudoLibrary.Models;
using JudoLibrary.Models.Moderation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
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
                    // * Identity seeding part -> we need Users to exist before submission coz they relate to them
                    // Get user manager service
                    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

                    // Create a test User
                    var testUser = new IdentityUser("test") {Email = "test@test.com"};
                    userManager.CreateAsync(testUser, "password").GetAwaiter().GetResult();

                    // Create a mod
                    var mod = new IdentityUser("mod") {Email = "mod@test.com"};
                    userManager.CreateAsync(mod, "password").GetAwaiter().GetResult();
                    // Adds the specified claim to the user(Mod) with providing claim type and value that we specified in
                    // our custom policy
                    userManager.AddClaimAsync(mod,
                            new Claim(JudoLibraryConstants.Claims.Role, JudoLibraryConstants.Roles.Mod))
                        .GetAwaiter()
                        .GetResult();

                    // Seeding User
                    context.Add(new User
                    {
                        Id = testUser.Id,
                        Username = testUser.UserName,
                        Image = "https://localhost:5001/api/files/image/user.jpg"
                    });

                    // Seeding Categories
                    context.Add(new Category
                    {
                        Id = "nage-waza", Name = "Nage Waza",
                        Description = "Throwing Techniques"
                    });
                    context.Add(new Category
                    {
                        Id = "katame-waza", Name = "Katame Waza",
                        Description = "Grappling Techniques"
                    });
                    context.Add(new Category
                    {
                        Id = "atemi-waza", Name = "Atemi Waza",
                        Description = "Body Striking Techniques"
                    });
                    context.Add(new Category
                    {
                        Id = "uke-waza", Name = "Uke Waza",
                        Description = "Blocks And Parries"
                    });

                    // Seeding SubCategories
                    context.Add(new SubCategory
                    {
                        Id = "te-waza", Name = "Te Waza",
                        Description = "Hand throwing techniques", CategoryId = "nage-waza"
                    });

                    context.Add(new SubCategory
                    {
                        Id = "koshi-waza", Name = "Koshi Waza",
                        Description = "Hip throwing techniques", CategoryId = "nage-waza"
                    });

                    context.Add(new SubCategory
                    {
                        Id = "ashi-waza", Name = "Ashi Waza",
                        Description = "Foot throwing techniques", CategoryId = "nage-waza"
                    });

                    context.Add(new SubCategory
                    {
                        Id = "sutemi-waza", Name = "Sutemi waza",
                        Description = "Sacrifice techniques", CategoryId = "nage-waza"
                    });

                    context.Add(new SubCategory
                    {
                        Id = "osaekomi-waza", Name = "Osaekomi waza ",
                        Description = "Pins or matholds", CategoryId = "katame-waza"
                    });

                    // Seeding Techniques
                    context.Add(new Technique
                    {
                        Id = 1, UserId = testUser.Id, Version = 1, Active = true, Slug = "kouchi-gari", Name = "Kouchi gari",
                        Description = "Small inner reap", Category = "nage-waza", SubCategory = "ashi-waza"
                    });

                    context.Add(new Technique
                    {
                        Id = 2, UserId = testUser.Id, Version = 1, Active = true, Slug = "ushiro-goshi", Name = "Ushiro goshi",
                        Description = "Rear hip throw", Category = "nage-waza", SubCategory = "koshi-waza"
                    });

                    context.Add(new Technique
                    {
                        Id = 3, UserId = testUser.Id, Version = 1, Active = true, Slug = "tani-otoshi", Name = "Tani otoshi",
                        Description = "Valley drop", Category = "nage-waza", SubCategory = "sutemi-waza"
                    });

                    context.Add(new Technique
                    {
                        Id = 4, UserId = testUser.Id, Version = 1, Active = true, Slug = "kesa-gatame", Name = "Kesa-gatame",
                        Description = "Scarf hold", Category = "katame-waza", SubCategory = "osaekomi-waza"
                    });

                    // * Main Technique *
                    context.Add(new Technique
                    {
                        Id = 5, UserId = testUser.Id, Version = 1, Active = true, Slug = "seoi-nage", Name = "Seoi Nage",
                        Description = "Shoulder throw", Category = "nage-waza", SubCategory = "te-waza",
                        SetUpAttacks = new List<TechniqueSetupAttack>
                        {
                            new TechniqueSetupAttack {TechniqueId = 5, SetUpAttackId = 1, Active = true}
                        },
                        FollowUpAttacks = new List<TechniqueFollowupAttack>
                        {
                            new TechniqueFollowupAttack {TechniqueId = 5, FollowUpAttackId = 6, Active = true}
                        },
                        Counters = new List<TechniqueCounter>
                        {
                            new TechniqueCounter {TechniqueId = 5, CounterId = 2, Active = true},
                            new TechniqueCounter {TechniqueId = 5, CounterId = 3, Active = true}
                        }
                    });
                    
                    context.Add(new Technique
                    {
                        Id = 6, UserId = testUser.Id, Version = 1, Active = true, Slug = "osoto-gari", Name = "Osoto gari",
                        Description = "Major Outer Reaping",
                        Category = "nage-waza", SubCategory = "ashi-waza",
                        SetUpAttacks = new List<TechniqueSetupAttack>
                        {
                            new TechniqueSetupAttack {TechniqueId = 6, SetUpAttackId = 3, Active = true},
                        },
                        FollowUpAttacks = new List<TechniqueFollowupAttack>
                        {
                            new TechniqueFollowupAttack {TechniqueId = 6, FollowUpAttackId = 1, Active = true}
                        },
                        Counters = new List<TechniqueCounter>
                        {
                            new TechniqueCounter {TechniqueId = 6, CounterId = 2, Active = true}
                        }
                    });

                    // Seeding submissions
                    context.Add(new Submission
                    {
                        TechniqueId = "seoi-nage",
                        Video = new Video
                        {
                            ThumbLink = "https://localhost:5001/api/files/image/seoi.jpg",
                            VideoLink = "https://localhost:5001/api/files/video/seoi.mp4"
                        },
                        Description = "This Seoi nage was very hard to pull of...",
                        VideoProcessed = true,
                        UserId = testUser.Id,
                    });

                    context.Add(new Submission
                    {
                        TechniqueId = "osoto-gari",
                        Video = new Video
                        {
                            ThumbLink = "https://localhost:5001/api/files/image/osoto.jpg",
                            VideoLink = "https://localhost:5001/api/files/video/osoto.mp4"
                        },
                        Description = "Demonstration of Osoto Gari",
                        VideoProcessed = true,
                        UserId = testUser.Id,
                    });

                    // Seeding moderation items
                    context.Add(new ModerationItem
                    {
                        Target = 6,
                        Type = ModerationTypes.Technique
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