using JudoLibrary.Models.Abstractions;

namespace JudoLibrary.Models
{
    // User will vote for particular submission
    public class SubmissionVote : Vote
    {
        public int SubmissionId { get; set; } 
        public Submission Submission { get; set; }
        public int Value { get; set; }
    }
}