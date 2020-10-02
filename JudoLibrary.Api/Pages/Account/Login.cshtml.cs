using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace JudoLibrary.Api.Pages.Account
{
    // Class for trying to Sign In / Log in user
    public class Login : PageModel
    {
        // Binds properties from form, something like v-model, contains props from LoginForm class
        [BindProperty]
        public LoginForm Form { get; set; }
        
        // http://localhost:5000/Account/Login?ReturnUrl=...
        // return Url gonna get populated by IS4 and authorization endpoint, needs to be named returnUrl otherwise wont work
        public void OnGet(string returnUrl)
        {
            // OnGet init LoginForm
            Form = new LoginForm()
            {
                // Assign to from return url, the url that's populated by IS4
                ReturnUrl = returnUrl
            };
        }
        
        // Method for loging in -> when we submit the form => press the Log In button
        // DI at method level, injecting SignInManager for User
        // SignInManager is a service -> come from Startup -> AddIdentity
        public async Task<IActionResult> OnPostAsync([FromServices] SignInManager<IdentityUser> signInManager)
        {
            // If login form is invalid, return that page again => user did not provide valid username || password
            if (!ModelState.IsValid)
                return Page();
            
            // Attempts to sign in the specified userName and password combination, if we close it's gonna persist, no lockout on failures
            // Providing username and pass from form to -> PasswordSignInAsync
            var signInResult = await signInManager.PasswordSignInAsync(Form.Username, Form.Password, true, false);
            
            if (signInResult.Succeeded)
            {
                // If signInResult succeeded we want to pop user back on from where he came from, returnUrl contains all the info
                // that server need to redirect us back to the App -> ReturnUrl is callback
                return Redirect(Form.ReturnUrl);
            }

            return Page();
        }
        
        // Class that represents login form -> input fields
        public class LoginForm
        {
            [Required]
            public string ReturnUrl { get; set; }

            [Required]
            public string Username { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }
        }
    }
}