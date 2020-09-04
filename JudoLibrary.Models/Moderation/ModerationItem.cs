using System.Collections.Generic;

namespace JudoLibrary.Models.Moderation
{
    // Container for moderated items => used for separation concerns
    public class ModerationItem : BaseModel<int>
    {
        // Target is pointer to some technique for example that we wanna moderate => where it is
        public string Target { get; set; }
        
        // Type represents -> what is the type of target that we are moderating => what it is
        public string Type { get; set; }

        // Comments for moderation => notes?, moderation item can have Many Comments
        public IList<Comment> Comments { get; set; } = new List<Comment>();

        // Moderation item can have Many Reviews
        public IList<Review> Reviews { get; set; } = new List<Review>();
    }
}