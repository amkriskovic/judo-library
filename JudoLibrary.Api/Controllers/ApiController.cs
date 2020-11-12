using System.Linq;
using System.Security.Claims;
using IdentityModel;
using Microsoft.AspNetCore.Mvc;

namespace JudoLibrary.Api.Controllers
{
    [ApiController]
    public class ApiController : ControllerBase
    {
        // UserId & PreferredUsername "getters"
        protected string UserId => GetClaim(ClaimTypes.NameIdentifier) ?? GetClaim(JwtClaimTypes.Subject);
        protected string PreferredUsername => GetClaim(ClaimTypes.Name) ?? GetClaim(JwtClaimTypes.PreferredUserName);

        protected bool IsMod => User.HasClaim(JudoLibraryConstants.Claims.Role, JudoLibraryConstants.Roles.Mod);
        
        // Returns Claim value based on claim type provided as parameter
        private string GetClaim(string claimType)
        {
            // Gets the ClaimsPrincipal for User associated with the executing action.

            // Iterate over Users claims and FOD where Claim type is equal to JWT Subject which is
            // Unique Identifier for the End-User at the Issuer => grab the Value of the Claim => "sub": "value"
            return User.Claims
                .FirstOrDefault(c => c.Type.Equals(claimType))?.Value; 
        }
    }
}