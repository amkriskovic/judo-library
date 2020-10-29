using JudoLibrary.Models.Abstractions;

namespace JudoLibrary.Models
{
    // User will vote for particular submission
    public class SubmissionVote : BaseModel<int>
    {
        public string SubmissionId { get; set; } 
        public Submission Submission { get; set; } 
        
        public string UserId { get; set; } 
        public User User { get; set; } 
    }
}