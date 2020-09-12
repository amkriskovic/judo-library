using System.Threading.Channels;
using IdentityServer4;
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
            // Adding separate Db Context for our identity(User management) layer, default is: IdentityUser, IdentityRole, string
            services.AddDbContext<IdentityDbContext>(config =>
            {
                // Place where we are going to store User records
                config.UseInMemoryDatabase("DevIdentity");
            });
            
            // Adding identity to services for managing User/role information
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
                // Adds the default token providers used to generate tokens for reset passwords, change email...
                .AddDefaultTokenProviders(); 

            services.ConfigureApplicationCookie(config =>
            {
                // Set to cookie login path
                // They flow is as follows 
                // localhost:3000 -> click Login button
                // redirect to /connect/authorize, this has the equivalent of the [Authorize] attribute
                // redirect to /Account/Login because we are NOT authorized
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
                // Adding identity resources in memory -> resources are all those important data which needs to be protected
                // Identity resources: represent claims about a user like user ID, display name, email address etc…
                // An identity resource is a named group of claims that can be requested using the scope parameter.
                identityServiceBuilder.AddInMemoryIdentityResources(new IdentityResource[]
                {
                    // These are Scopes =>> area/category of information's that we can extract -> we can give access to client to req. those scopes
                    
                    // OpenId -> Informs the Authorization Server that the Client is making an OpenID Connect request. OpenID CR gives us ID Token
                    // and access Token, after exchanging for auth. code, and ID Token can be immediately consumed by application to understand who
                    // are you? And then the access token can be used to gather more information about the user e.g. (/connect/userinfo)
                    // ID Token === understand who the user is, | Access Token === is used for requesting API's
                    // OpenId contains well known openid configuration, which is called discovery document, which has all the information about IS4
                    new IdentityResources.OpenId(),
                    
                    // Profile scope is used to gather info about the User
                    // This scope value requests access to the End-User's default profile Claims, there is many of them.
                    new IdentityResources.Profile(),
                });
                
                // Client => thing that is receiving the tokens, tokens are gonna contain information, IS4 either gonna allow to client
                // to access scopes above or not

                // #1 The application opens a browser to send the user to the IS4, with ClientId, RedirectURI, Response type, scope...
                // #1 scope ->> determines what client can do (We need to specify those scopes to Client) [FC]
                // #2 The user sees the authorization prompt(consent) and approves the app’s request [FC]
                // #3 The user is redirected back to the application with an authorization code (in the query string) [FC]
                // #4 The application exchanges the authorization code by calling IS4 for an access token [BC] (Secret key)
                // #5 Application now can talk to resource sever/API with access token [BC] ->> It's scoped to what we specify in scope
                
                // Adding in mem client to identity service builder
                identityServiceBuilder.AddInMemoryClients(new Client[]
                {
                    // Instantiating Client -> This is our Vue application
                    new Client
                    {
                        // FC == Front channel -> not that secure ->> browser => used to interact with a User
                        // BC == Back channel -> secure ->> server
                        
                        // Client id, mimicking our Vue App client name
                        // ClientId is passed with initial request [FC] -> not sensitive -> identifies our Judo App,
                        // the client_id is a public identifier for App.
                        ClientId = "web-client",

                        // Specifying authorization code flow, provides a way to retrieve tokens on a back-channel as opposed to the browser
                        // front-channel. It also support client authentication.
                        
                        // Grant types are a way to specify how a client wants to interact with IdentityServer
                        // Grant type refers to the way an application gets an access token.
                        AllowedGrantTypes = GrantTypes.Code,

                        // Where do we redirect after they logIn
                        RedirectUris = new string[] {"http://localhost:3000"},

                        // Where do we redirect after they logOut
                        PostLogoutRedirectUris = new string[] {"http://localhost:3000"},

                        // CORS origins -> coz ppl are gonna come from different domains
                        AllowedCorsOrigins = new string[] {"http://localhost:3000"},
                        
                        // We need to tell in client configuration that this Client is allowed to grab specified scopes that we defined in
                        // AddInMemoryIdentityResources on IS4 side
                        AllowedScopes = new string[]
                        {
                            // Passing allowed scopes that client can request from IS4
                            IdentityServerConstants.StandardScopes.OpenId,
                            IdentityServerConstants.StandardScopes.Profile
                        },

                        // This is to allow code flow thorough the browser
                        // Adds code & code_verifier to http://localhost:5000/connect/token, in form data response
                        RequirePkce = true,
                        
                        // Allowing transmitting access tokens via the browser channel
                        AllowAccessTokensViaBrowser = true,

                        // Disable consent screen -> when some web page asks as to grant info about email...
                        // Useful for third party clients, our client knows everything that he needs to know
                        RequireConsent = false,

                        // Our client does not need a secret to request tokens from the token endpoint
                        RequireClientSecret = false
                    },
                });
                
                // Adding sign in credentials to IS4 builder,
                // Creates temporary key material at startup time. This is for dev scenarios.
                // The generated key will be persisted in the local directory by default.
                // JWT has signatures, in order to create signature we need some secure keys
                // Sets the temporary signing credential
                identityServiceBuilder.AddDeveloperSigningCredential();
            }
            // * 
        }
    }
}