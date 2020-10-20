using JudoLibrary.Models.Abstractions;

namespace JudoLibrary.Models.Moderation
{
    public class Review : BaseModel<int>
    {
        // Review can have One Moderation Item => belongs to
        public int ModerationItemId { get; set; }
        public ModerationItem ModerationItem { get; set; }
        
        // This is explanation for your decision of the review when we approve/reject/put on wait,
        // adding some note to review(status) => explanation of the review that reviewer is giving
        public string Comment { get; set; }

        // Status of the review => Each Review can have One Status
        public ReviewStatus Status { get; set; }
    }

    // Types of reviews
    public enum ReviewStatus
    {
        Approved = 0,
        Rejected = 1,
        Waiting = 2
    }
}