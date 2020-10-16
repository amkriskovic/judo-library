using System.Collections.Generic;
using JudoLibrary.Models.Abstractions;

namespace JudoLibrary.Models
{
    // This represent User PROFILE
    public class User : TemporalModel
    {
        // Id that we are gonna take/assign from our "sub" Claim
        // new => overriding int Id from TemporalModel
        public new string Id { get; set; }
        public string Username { get; set; }

        // Represents file input for image
        public string Image { get; set; }

        // One User can have Many Submissions -> init so the list is always present
        public IList<Submission> Submissions { get; set; } = new List<Submission>();
    }
}