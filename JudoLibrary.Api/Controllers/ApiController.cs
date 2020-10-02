using System.Linq;
using IdentityModel;
using Microsoft.AspNetCore.Mvc;

namespace JudoLibrary.Api.Controllers
{
    [ApiController]
    public class ApiController : ControllerBase
    {
        // UserId & PreferredUsername "getters"
        protected string UserId => GetClaim(JwtClaimTypes.Subject);
        protected string PreferredUsername => GetClaim(JwtClaimTypes.PreferredUserName);
        
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