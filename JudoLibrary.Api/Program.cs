using System;
using System.Collections.Generic;
using System.Linq;
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
                    var fakeCounter = 20;

                    // * Identity seeding part -> we need Users to exist before submission coz they relate to them
                    // Get user manager service
                    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

                    // Created a test User
                    var testUser = new IdentityUser("test") {Id = "test_user_id", Email = "test@test.com"};
                    userManager.CreateAsync(testUser, "password").GetAwaiter().GetResult();
                    
                    // Seeding User
                    context.Add(new User
                    {
                        Id = testUser.Id,
                        Username = testUser.UserName,
                        Image = "https://localhost:5001/api/files/image/user.jpg"
                    });

                    // Creating(Blueprinting) fake users based on counter above
                    var fakeUsers = Enumerable.Range(0, fakeCounter)
                        .Select(i => new IdentityUser($"fake{i}") {Id = $"fake_{i}_id", Email = $"fake{i}@test.com"})
                        .ToList();

                    // Loop over fake users and seed them
                    foreach (var fakeUser in fakeUsers)
                    {
                        userManager.CreateAsync(fakeUser, "password").GetAwaiter().GetResult();

                        context.Add(new User
                        {
                            Id = fakeUser.Id,
                            Username = fakeUser.UserName
                        });
                    }

                    // Created a mod
                    var mod = new IdentityUser("mod") {Id = "mod_user_id", Email = "mod@test.com"};
                    userManager.CreateAsync(mod, "password").GetAwaiter().GetResult();
                    // Adds the specified claim to the user(Mod) with providing claim type and value that we specified in
                    // our custom policy
                    userManager.AddClaimAsync(mod,
                            new Claim(JudoLibraryConstants.Claims.Role, JudoLibraryConstants.Roles.Mod))
                        .GetAwaiter()
                        .GetResult();
                    
                    // Seeding Mod
                    context.Add(new User
                    {
                        Id = mod.Id,
                        Username = mod.UserName,
                        Image = "https://localhost:5001/api/files/image/judge.jpg"
                    });
                    
                    // Seeding Categories
                    var categories = new List<Category>()
                    {
                        new Category
                        {
                            Id = 1,
                            Slug = "nage-waza", 
                            Name = "Nage Waza",
                            Description = "Throwing Techniques" ,
                            State = VersionState.Live
                        },
                        new Category
                        {
                            Id = 2,
                            Slug = "katame-waza", 
                            Name = "Katame Waza",
                            Description = "Grappling Techniques",
                            State = VersionState.Live
                        },
                        new Category
                        {
                            Id = 3,
                            Slug = "atemi-waza", 
                            Name = "Atemi Waza",
                            Description = "Body Striking Techniques",
                            State = VersionState.Live
                        },
                    };
                    context.AddRange(categories);

                    context.Add(new Category
                    {
                        Id = 4,
                        Slug = "mod", 
                        Name = "Mod",
                        Description = "Category under Moderation",
                        State = VersionState.Live,
                    });


                    // Seeding SubCategories
                    var subcategories = new List<SubCategory>()
                    {
                        new SubCategory
                        {
                            Id = 1, Slug = "te-waza", Name = "Te Waza",
                            Description = "Hand throwing techniques", CategoryId = "nage-waza",
                            State = VersionState.Live
                        },
                        new SubCategory
                        {
                            Id = 2, Slug = "koshi-waza", Name = "Koshi Waza",
                            Description = "Hip throwing techniques", CategoryId = "nage-waza",
                            State = VersionState.Live
                        },
                        new SubCategory
                        {
                            Id = 3, Slug = "ashi-waza", Name = "Ashi Waza",
                            Description = "Foot throwing techniques", CategoryId = "nage-waza",
                            State = VersionState.Live
                        },
                        new SubCategory
                        {
                            Id = 4, Slug = "sutemi-waza", Name = "Sutemi waza",
                            Description = "Sacrifice techniques", CategoryId = "nage-waza",
                            State = VersionState.Live
                        },
                        new SubCategory
                        {
                            Id = 5, Slug = "osaekomi-waza", Name = "Osaekomi waza ",
                            Description = "Pins or matholds", CategoryId = "katame-waza",
                            State = VersionState.Live
                        } 
                    };
                    context.AddRange(subcategories);


                    // Seeding Techniques
                    context.Add(new Technique
                    {
                        Id = 1, UserId = testUser.Id, Version = 1, State = VersionState.Live, Slug = "kouchi-gari",
                        Name = "Kouchi gari",
                        Description = "Small inner reap", 
                        TechniqueCategories = new List<TechniqueCategory>
                        {
                            new TechniqueCategory {CategoryId = 1, Active = true}
                        }, 
                        TechniqueSubCategories = new List<TechniqueSubCategory>
                        {
                            new TechniqueSubCategory {SubCategoryId = 3, Active = true}
                        }
                    });

                    context.Add(new Technique
                    {
                        Id = 2, UserId = testUser.Id, Version = 1, State = VersionState.Live, Slug = "ushiro-goshi",
                        Name = "Ushiro goshi",
                        Description = "Rear hip throw", 
                        TechniqueCategories = new List<TechniqueCategory>
                        {
                            new TechniqueCategory {CategoryId = 1, Active = true}
                        },
                        TechniqueSubCategories = new List<TechniqueSubCategory>
                        {
                            new TechniqueSubCategory {SubCategoryId = 2, Active = true}
                        }
                    });

                    context.Add(new Technique
                    {
                        Id = 3, UserId = testUser.Id, Version = 1, State = VersionState.Live, Slug = "tani-otoshi",
                        Name = "Tani otoshi",
                        Description = "Valley drop", 
                        TechniqueCategories = new List<TechniqueCategory>
                        {
                            new TechniqueCategory {CategoryId = 1, Active = true}
                        },
                        TechniqueSubCategories = new List<TechniqueSubCategory>
                        {
                            new TechniqueSubCategory {SubCategoryId = 4, Active = true}
                        }
                    });

                    context.Add(new Technique
                    {
                        Id = 4, UserId = testUser.Id, Version = 1, State = VersionState.Live, Slug = "kesa-gatame",
                        Name = "Kesa-gatame",
                        Description = "Scarf hold", 
                        TechniqueCategories = new List<TechniqueCategory>
                        {
                            new TechniqueCategory {CategoryId = 2, Active = true}
                        },
                        TechniqueSubCategories = new List<TechniqueSubCategory>
                        {
                            new TechniqueSubCategory {SubCategoryId = 5, Active = true}
                        }
                    });

                    // * Main Technique *
                    context.Add(new Technique
                    {
                        Id = 5, UserId = testUser.Id, Version = 1, State = VersionState.Live, Slug = "seoi-nage",
                        Name = "Seoi Nage",
                        Description = "Shoulder throw", 
                        TechniqueCategories = new List<TechniqueCategory>
                        {
                            new TechniqueCategory {CategoryId = 1, Active = true}
                        },
                        TechniqueSubCategories = new List<TechniqueSubCategory>
                        {
                            new TechniqueSubCategory {SubCategoryId = 1, Active = true}
                        },
                        SetUpAttacks = new List<TechniqueSetupAttack>
                        {
                            new TechniqueSetupAttack {TechniqueId = 5, SetUpAttackId = 1, Active = true}
                        },
                        FollowUpAttacks = new List<TechniqueFollowupAttack>
                        {
                            new TechniqueFollowupAttack {TechniqueId = 5, FollowUpAttackId = 2, Active = true}
                        },
                        Counters = new List<TechniqueCounter>
                        {
                            new TechniqueCounter {TechniqueId = 5, CounterId = 2, Active = true},
                            new TechniqueCounter {TechniqueId = 5, CounterId = 3, Active = true}
                        }
                    });

                    context.Add(new Technique
                    {
                        Id = 6, UserId = testUser.Id, Version = 1, State = VersionState.Live, Slug = "osoto-gari",
                        Name = "Osoto gari",
                        Description = "Major Outer Reaping",
                        TechniqueCategories = new List<TechniqueCategory>
                        {
                            new TechniqueCategory {CategoryId = 1, Active = true}
                        },
                        TechniqueSubCategories = new List<TechniqueSubCategory>
                        {
                            new TechniqueSubCategory {SubCategoryId = 3, Active = true}
                        },
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
                        Votes = new List<SubmissionVote>
                        {
                            new SubmissionVote
                            {
                                UserId = testUser.Id,
                                Value = 1,
                            }
                        }
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
                        Type = ModerationTypes.Technique,
                        UserId = testUser.Id
                    });
                    
                    context.Add(new ModerationItem
                    {
                        Target = 4,
                        Type = ModerationTypes.Category,
                        UserId = testUser.Id
                    });

                    // Saving changes to in-memory DB
                    context.SaveChanges();

                    // Created fakeCounter amount of fake submissions
                    for (int i = 1; i <= fakeCounter; i++)
                    {
                        context.Add(new Submission
                        {
                            TechniqueId = "osoto-gari",
                            Description = $"Fake submission {i}",
                            Video = new Video
                            {
                                ThumbLink = "https://localhost:5001/api/files/image/osoto.jpg",
                                VideoLink = "https://localhost:5001/api/files/video/osoto.mp4"
                            },
                            VideoProcessed = true,
                            UserId = testUser.Id,

                            Created = DateTime.UtcNow.AddDays(-i),
                            Votes = Enumerable
                                .Range(0, i)
                                .Select(ii => new SubmissionVote
                                {
                                    UserId = fakeUsers[ii].Id,
                                    Value = 1,
                                })
                                .ToList(),
                            Comments = Enumerable
                                .Range(0, fakeCounter)
                                .Select(ii => new Comment
                                {
                                    UserId = fakeUsers[ii].Id,
                                    Content = $"Main Comment {ii}",
                                    HtmlContent = $"Main Comment {ii}",
                                    Replies = Enumerable
                                        .Range(0, fakeCounter)
                                        .Select(iii => new Comment
                                        {
                                            UserId = fakeUsers[iii].Id,
                                            Content = $"Reply to Comment {iii}",
                                            HtmlContent = $"Reply to Comment {iii}",
                                        })
                                        .ToList(),
                                })
                                .ToList(),
                        });
                    }

                    // Save fake Submissions to DB
                    context.SaveChanges();

                    for (int i = 0; i < fakeCounter; i++)
                    {
                        var technique = new Technique
                        {
                            UserId = testUser.Id, 
                            Version = 1, 
                            State = VersionState.Live, 
                            Slug = $"fake-technique-{i}",
                            Name = $"Fake Technique que {i}",
                            Description = $"This is a fake technique # {i}", 
                            TechniqueCategories = new List<TechniqueCategory>
                            {
                              new TechniqueCategory {CategoryId = categories[i % categories.Count].Id, Active = true}  
                            },
                            TechniqueSubCategories = new List<TechniqueSubCategory>
                            {
                                new TechniqueSubCategory {SubCategoryId = subcategories[i % subcategories.Count].Id, Active = true}  
                            },
                            // SetUpAttacks = new List<TechniqueSetupAttack>
                            // {
                            //     new TechniqueSetupAttack {TechniqueId = 5, SetUpAttackId = 1, State = VersionState.Live}
                            // },
                            // FollowUpAttacks = new List<TechniqueFollowupAttack>
                            // {
                            //     new TechniqueFollowupAttack {TechniqueId = 5, FollowUpAttackId = 6, State = VersionState.Live}
                            // },
                            // Counters = new List<TechniqueCounter>
                            // {
                            //     new TechniqueCounter {TechniqueId = 5, CounterId = 2, State = VersionState.Live},
                            //     new TechniqueCounter {TechniqueId = 5, CounterId = 3, State = VersionState.Live}
                            // }
                        };
                        context.Add(technique);
                        context.Add(new Submission
                        {
                            TechniqueId = technique.Slug,
                            Description = $"Fake submission |||| {i}",
                            Video = new Video
                            {
                                ThumbLink = "https://localhost:5001/api/files/image/three.jpg",
                                VideoLink = "https://localhost:5001/api/files/video/three.mp4"
                            },
                            VideoProcessed = true,
                            UserId = testUser.Id,

                            Created = DateTime.UtcNow.AddDays(-i),
                            Votes = Enumerable
                                .Range(0, i)
                                .Select(ii => new SubmissionVote
                                {
                                    UserId = fakeUsers[ii].Id,
                                    Value = 1,
                                })
                                .ToList(),
                        });
                        context.SaveChanges();
                    }
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