using JudoLibrary.Models.Moderation;

namespace JudoLibrary.Api.Form
{
    public class ReviewForm
    {
        // This is explanation for your decision of the review when we approve/reject/put on wait,
        // adding some note to review(status) => explanation of the review that reviewer is giving
        public string Comment { get; set; }

        // Status of the review => Each Review can have One Status
        public ReviewStatus Status { get; set; }
    }
}