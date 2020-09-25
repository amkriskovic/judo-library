using System.Security.Claims;
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
                .AddPolicy(AllCors, build => build
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

            // * Authentication part, Who are you?
            app.UseAuthentication();
            app.UseIdentityServer();

            // * Authorization part, are you allowed?
            app.UseAuthorization();

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
                
                // Set logout path
                config.LogoutPath = "/api/auth/logout";
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
                    // Scopes for User information

                    // OpenId -> Informs the Authorization Server that the Client is making an OpenID Connect request.
                    // OpenID CR gives us ID Token
                    // and access Token, after exchanging for auth. code, and ID Token can be immediately consumed by application
                    // to understand who
                    // are you? And then the access token can be used to gather more information about the user
                    // e.g. (/connect/userinfo)
                    // ID Token === understand who the user is, | Access Token === is used for requesting API's
                    // OpenId contains well known openid configuration, which is called discovery document, which has all the information about IS4
                    new IdentityResources.OpenId(),

                    // Profile scope is used to gather info about the User
                    // This scope value requests access to the End-User's default profile Claims, there is many of them.
                    new IdentityResources.Profile(),
                    
                    // Adding custom IdentityResource which we will include in ID Token -> Role
                    new IdentityResource(JudoLibraryConstants.IdentityResources.RoleScope, 
                        new string[] {JudoLibraryConstants.Claims.Role}), 
                });

                // Adding in memory API scopes that client can request -> hidden information
                identityServiceBuilder.AddInMemoryApiScopes(new ApiScope[]
                {
                    // Providing api scope with scope name -> "IdentityServerApi"
                    // This allows us to request this scope (Client), for IS4 means it's gonna be some resource behind this scope
                    // IdentityServerApi will be included in "scopes_supported" in /.well-known/openid-configuration
                    
                    // Adding ClaimType *Role* to our ApiScope ("claims_supported") -> after that it will be included in JWT access token
                    // -> putting that claim from
                    // Identity4 side to access token, it will be something like -> role: "Mod" in User's access token
                    
                    // The following scope definition tells the configuration system, that when a ScopeName -> IdentityServerApi scope gets granted,
                    // the Role claim should be added to the access token:
                    new ApiScope(IdentityServerConstants.LocalApi.ScopeName, new string[]{JudoLibraryConstants.Claims.Role})
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

                        // Where do we redirect after they log In
                        RedirectUris = new string[] {"https://localhost:3000/oidc/sign-in-callback.html"},

                        // Where do we redirect after they log Out
                        PostLogoutRedirectUris = new string[] {"https://localhost:3000"},

                        // CORS origins -> coz ppl are gonna come from different domains
                        AllowedCorsOrigins = new string[] {"https://localhost:3000"},

                        // We need to tell in client configuration that this Client is allowed to grab specified scopes that we defined in
                        // AddInMemoryIdentityResources on IS4 side
                        AllowedScopes = new string[]
                        {
                            // Passing allowed scopes that client can request from IS4
                            IdentityServerConstants.StandardScopes.OpenId,
                            IdentityServerConstants.StandardScopes.Profile,

                            // Adds IdentityServerApi to "scopes_supported"
                            IdentityServerConstants.LocalApi.ScopeName,
                            
                            JudoLibraryConstants.IdentityResources.RoleScope, // Custom
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

            // * Authentication & Authorization part of IS4 for accessing APIs, true for dev. and production
            // To enable token validation for local APIs, add the following to your IdentityServer startup
            services.AddLocalApiAuthentication();
            
            // Custom authorization for MOD role
            services.AddAuthorization(options =>
            {
                // Elevating default IS4 policy
                options.AddPolicy(JudoLibraryConstants.Policies.Mod, policy =>
                {
                    // Getting default IS4 policy
                    var IS4Policy = options.GetPolicy(IdentityServerConstants.LocalApi.PolicyName);
                    
                    // Our policy (Mod) + default policy
                    // Combines the specified policy into the current instance, adding to our (Mod) policy, default one
                    policy.Combine(IS4Policy);
                    
                    // Specifying required claims for this policy, Role claim type & allowed values for that role -> you need to be a Moderator
                    policy.RequireClaim(JudoLibraryConstants.Claims.Role, JudoLibraryConstants.Roles.Mod);
                });
            });

            // * 
        }

    }
    
    public struct JudoLibraryConstants
    {
        public struct Policies
        {
            public const string Mod = nameof(Mod);
        }
        
        public struct IdentityResources
        {
            public const string RoleScope = "role";
        }
        
        public struct Claims
        {
            public const string Role = "role";
        }
        
        public struct Roles
        {
            public const string Mod = nameof(Mod);
        }
    }
}