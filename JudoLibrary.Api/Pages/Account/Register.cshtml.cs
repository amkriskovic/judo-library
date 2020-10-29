using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace JudoLibrary.Api.Pages.Account
{
    public class Register : BasePage // Inherits from BasePage which inherits PageModel
    {
        // Binds properties from register form, something like v-model, contains props from RegisterForm class
        [BindProperty]
        public RegisterForm Form { get; set; }
        
        // http://localhost:5000/Account/Login?ReturnUrl=...
        // return Url gonna get populated by IS4 and authorization endpoint, needs to be named returnUrl otherwise wont work
        public void OnGet(string returnUrl)
        {
            // OnGet init RegisterForm
            Form = new RegisterForm()
            {
                // Assign to from return url, the url that's populated by IS4
                ReturnUrl = returnUrl
            };
        }
        
        // Method for registering in -> when we submit the form
        // DI at method level, injecting SignInManager & UserManager for User
        // UserManager is a service -> come from Startup -> AddIdentity
        // SignInManager is a service -> come from Startup -> AddIdentity
        public async Task<IActionResult> OnPostAsync(
            [FromServices] UserManager<IdentityUser> userManager,
            [FromServices] SignInManager<IdentityUser> signInManager
            )
        {
            // If register form is invalid, return that page again => user did not provide valid email || password
            if (!ModelState.IsValid)
                return Page();
            
            // Created a User, with Username -> what he typed in the form
            var user = new IdentityUser(Form.Username)
            {
                // Adding email from form to user object as-well
                Email = Form.Email
            };

            // Creates user in store with user manager, based on identity (username included) user and password from form
            var createUserResult = await userManager.CreateAsync(user, Form.Password);
            
            // If creation succeeded
            if (createUserResult.Succeeded)
            {
                // Sign the User in
                await signInManager.SignInAsync(user, true);
                
                // If signInResult succeeded we want to pop user back on from where he came from, returnUrl contains all the info
                // that server need to redirect us back to the App
                return Redirect(Form.ReturnUrl);
            }

            // Loop over errors in createUserResult -> when we try to create user and he fked up or didnt filled form correctly
            foreach (var error in createUserResult.Errors)
            {
                // Adding to our List of errors, error (it's description -> describes what error is) that popped for the reason above
                CustomErrors.Add(error.Description);
            }

            return Page();
        }
        
        // Class that represents register form -> input fields
        public class RegisterForm
        {
            [Required]
            public string ReturnUrl { get; set; }

            [Required]
            public string Email { get; set; }

            [Required]
            public string Username { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }
            
            [Required]
            [DataType(DataType.Password)]
            [Compare(nameof(Password))] // Compares Password above
            public string ConfirmPassword { get; set; }
        }
    }
}