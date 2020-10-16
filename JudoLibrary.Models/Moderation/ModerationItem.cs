using System.Collections.Generic;
using JudoLibrary.Models.Abstractions;

namespace JudoLibrary.Models.Moderation
{
    // Container for moderated items => used for separation concerns
    public class ModerationItem : TemporalModel
    {
        // Current version of particular thing(item) that we are moderating
        public int Current { get; set; }
        
        // Target version of particular thing(item) that we are moderating => 
        // With what version do we end up
        public int Target { get; set; }
        
        // Type represents => what is the type of target that we are moderating => what it is e.g. technique
        public string Type { get; set; }

        // Comments for moderation => notes?, moderation item can have Many Comments
        public IList<Comment> Comments { get; set; } = new List<Comment>();

        // Moderation item can have Many Reviews
        public IList<Review> Reviews { get; set; } = new List<Review>();
    }
}