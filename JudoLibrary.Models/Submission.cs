using System;
using System.Collections.Generic;
using JudoLibrary.Models.Abstractions;

namespace JudoLibrary.Models
{
    public class Submission : BaseModel<int>
    {
        public string TechniqueId { get; set; } // Submission can have One Technique => FKey
        public int VideoId { get; set; } // Submission can have One Video 
        public Video Video { get; set; } // Navigation prop
        public string Description { get; set; }
        public bool VideoProcessed { get; set; }
        
        public string UserId { get; set; } // Submission can have One User
        public User User { get; set; } // Navigation prop
        
        public IList<SubmissionVote> Votes { get; set; } = new List<SubmissionVote>();
        public IList<Comment> Comments { get; set; } = new List<Comment>();

    }
}