using JudoLibrary.Models.Abstractions;

namespace JudoLibrary.Models
{
    // User will vote for particular submission
    public class SubmissionMutable : Mutable
    {
        public int SubmissionId { get; set; } 
        public Submission Submission { get; set; }
        public int Value { get; set; }
    }
}