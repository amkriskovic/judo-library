using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace JudoLibrary.Api.Pages
{
    public class BasePage : PageModel
    {
        // List of string that will be container for custom errors
        public IList<string> CustomErrors { get; set; } = new List<string>();
    }
}