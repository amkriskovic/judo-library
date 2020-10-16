using System.Collections.Generic;
using JudoLibrary.Models.Abstractions;
using JudoLibrary.Models.Moderation;

namespace JudoLibrary.Models
{
    // Self referencing table
    public class Comment : TemporalModel
    {
        public string Content { get; set; }
        public string HtmlContent { get; set; }

        // Comment can have One Moderation Item
        // This is moderation item for comment -> ability for moderate comment
        public int? ModerationItemId { get; set; }
        public ModerationItem ModerationItem { get; set; }

        // Self referencing property which points to Comment -> Id
        // This is the parent comment -> used so we can have replies to that comment
        // ? => Id doesnt have to be present =>> comment doesnt need to have parent comment to exist
        public int? ParentId { get; set; } // ParentId is referencing Comments -> Id, and can have Zero or One of that
        public Comment Parent { get; set; } // Navigation prop -> This corresponds to many

        // Comment can have Many Replies
        // This is the child comment/s -> reply/ies to parent comment
        public IList<Comment> Replies { get; set; } = new List<Comment>();
    }
}