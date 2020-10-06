using System.Collections.Generic;

namespace JudoLibrary.Models
{
    // This represent User PROFILE
    public class User : BaseModel<string>
    {
        public string Username { get; set; }

        // Represents file input for image
        public string Image { get; set; }

        // One User can have Many Submissions -> init so the list is always present
        public IList<Submission> Submissions { get; set; } = new List<Submission>();
    }
}