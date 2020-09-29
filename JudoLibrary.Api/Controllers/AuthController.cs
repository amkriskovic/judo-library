using System.Threading.Tasks;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace JudoLibrary.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        // #1 After we click logout -> /connect/endsession end point is called from oidc well known document, which passes id_token_hint
        // and post_logout_redirect_uri

        // #2 After that this endpoint is called (/api/auth/logout) with logout_id, which is id of this client
        [HttpGet("logout")]
        // Method for logging out User, logoutId for IS4 coz there can be multiple clients, OIDC will manage redirect & logout
        // SignInManager DI at method level => used for signing out User
        // IIdentityServerInteractionService DI at method level => used for discovering post logout redirect URI
        public async Task<IActionResult> Logout(string logoutId,
            [FromServices] SignInManager<IdentityUser> signInManager,
            [FromServices] IIdentityServerInteractionService interactionService)
        {
            // # 1 Sign Out
            // Signs the current user OUT of the application.
            await signInManager.SignOutAsync();

            // # 2 Where do we wanna resume after signing out
            // Gets the logout context, need to provide logoutId
            var logoutRequest = await interactionService.GetLogoutContextAsync(logoutId);
            
            // If logoutRequest -> (post logout redirect URI) is null or empty
            if (string.IsNullOrEmpty(logoutRequest.PostLogoutRedirectUri))
            {
                // Return bad request
                return BadRequest();
            }

            // If it's not null or empty, => Gets or sets the post logout redirect URI.
            return Redirect(logoutRequest.PostLogoutRedirectUri);
        }
    }
}