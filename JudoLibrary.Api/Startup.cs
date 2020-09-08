using System.Threading.Channels;
using IdentityServer4.Models;
using JudoLibrary.Api.BackgroundServices;
using JudoLibrary.Api.BackgroundServices.VideoEditing;
using JudoLibrary.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
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
        private readonly IWebHostEnvironment _env;

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // Opening Session with Postgres SQL
            // services.AddDbContext<AppDbContext>(options => 
            //     options.UseNpgsql(_configuration.GetConnectionString("JudoLibrary")));
            
            // Adding service for using EF In-Memory database
            services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("Dev"));
            
            // Calling our custom identity func for setting up Identity & IS4
            AddIdentity(services);
            
            // Adds support for using controllers
            services.AddControllers();
            
            // Adds support for using razor pages
            services.AddRazorPages();
            
            // Adding hosted service for VideoEditingBackgroundService which implements BackgroundService <- IHostedService
            services.AddHostedService<VideoEditingBackgroundService>();
            
            // Adding singleton service as Channel of type <EditVideoMessage>, _ without service provider
            services.AddSingleton(_ => Channel.CreateUnbounded<EditVideoMessage>());
            services.AddSingleton<VideoManager>();
            
            services.AddControllersWithViews()
                .AddNewtonsoftJson(options =>
                    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                );

            // Service for CORS mechanism, with current policy, everything is accepted
            services.AddCors(options => options
                .AddPolicy(AllCors,build => build
                    .AllowAnyHeader()
                    .AllowAnyOrigin()
                    .AllowAnyMethod()));
        }

        public void Configure(IApplicationBuilder app)
        {
            if (_env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Middleware func for CORS mechanism
            app.UseCors(AllCors);
            
            // Support for routing
            app.UseRouting();
            
            // * Authentication part
            app.UseAuthentication();
            app.UseIdentityServer();

            // * Authorization part

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                
                // Mapping razor pages endpoints
                endpoints.MapRazorPages();
            });
        }

        // * Custom function for setting up identity -> |Authentication| part, this is gonna get injected to middleware
        private void AddIdentity(IServiceCollection services)
        {
            // * Identity layer -> user records -> storing it in DB
            // Base class for the Entity Framework database context used for identity.
            // Adding separate db context for our identity, default is: IdentityUser, IdentityRole, string
            services.AddDbContext<IdentityDbContext>(config =>
            {
                // Place where we are going to store user records
                config.UseInMemoryDatabase("DevIdentity");
            });
            
            // Adding identity to services for managing user/role information
            services.AddIdentity<IdentityUser, IdentityRole>(options =>
                {
                    if (_env.IsDevelopment())
                    {
                        // Specifying some password options
                        options.Password.RequireDigit = false;
                        options.Password.RequiredLength = 4;
                        options.Password.RequireLowercase = false;
                        options.Password.RequireUppercase = false;
                        options.Password.RequireNonAlphanumeric = false;
                    }
                    else
                    {
                        // todo: configure for production
                    }
                })
                // Wire up these services to DB (EF Store), connecting IdentityDbContext to DB
                .AddEntityFrameworkStores<IdentityDbContext>()
                .AddDefaultTokenProviders(); // -> key that going to be used when gen. passwords...

            services.ConfigureApplicationCookie(config =>
            {
                // Set to cookie login path
                config.LoginPath = "/Account/Login";
            });
            // * 
            
            // * Identity Server layer (IS4)
            // Create identity server builder -> adding to services
            var identityServiceBuilder = services.AddIdentityServer();
            
            // # We need to hook up IS4 with Identity(.NET Core), provide IdentityUser
            identityServiceBuilder.AddAspNetIdentity<IdentityUser>();

            if (_env.IsDevelopment())
            {
                // Adding to mem identity resources -> resources are all those important data which are protectable
                identityServiceBuilder.AddInMemoryIdentityResources(new IdentityResource[]
                {
                    // Scopes =>> area/category of information's that you want to extract
                    // OpenId -> This document contains information such as the location of various endpoints (for example, the token endpoint and the end session endpoint), the grant types the provider supports, the scopes it can authorize, and so on,
                    // Profile -> info about the user
                    new IdentityResources.OpenId(),
                    new IdentityResources.Profile(),
                });
                
                // Client => thing that is receiving the tokens, tokens are gonna contain information, IS4 either gonna allow to client
                // to access scopes above or not
                
                // Adding in mem client to builder
                identityServiceBuilder.AddInMemoryClients(new Client[]
                {
                    // Client -> Vue configuration
                    new Client
                    {
                        // Client id, mimicking our Vue client name
                        ClientId = "web-client",

                        // Specifying code flow as grant type
                        // “Grant Type” refers to the way an application gets an access token.
                        // Code GT -> first requiring the app launch a browser to begin the flow.

                        //-The application opens a browser to send the user to the OAuth server
                        //-The user sees the authorization prompt and approves the app’s request
                        //-The user is redirected back to the application with an authorization code in the query string
                        //-The application exchanges the authorization code for an access token
                        AllowedGrantTypes = GrantTypes.Code,

                        // Where do we redirect after they logIn
                        RedirectUris = new string[] {"http://localhost:3000"},

                        // Where do we redirect after they logOut
                        PostLogoutRedirectUris = new string[] {"http://localhost:3000"},

                        // CORS origins -> coz ppl are gonna come from different domains
                        AllowedCorsOrigins = new string[] {"http://localhost:3000"},

                        // This is to allow code flow thorough the browser
                        RequirePkce = true,
                        AllowAccessTokensViaBrowser = true,

                        // Disable consent screen -> when some web page asks as to grant info about email...
                        RequireConsent = false,

                        // Our client cannot hold the secret -> it's located in the users browser
                        RequireClientSecret = false
                    },
                });
                
                // Adding sign in credentials to IS4 builder, JWT has signatures, in order to create signature we need some secure keys
                identityServiceBuilder.AddDeveloperSigningCredential();
            }
            // * 
        }
    }
}