namespace JudoLibrary.Models
{
    public class Submission : BaseModel<int>
    {
        public int VideoId { get; set; } // Submission can have One Video 
        public Video Video { get; set; } // Navigation prop
        public string Description { get; set; }
        public bool VideoProcessed { get; set; }
        public string TechniqueId { get; set; } // Submission can have One Technique
        
        public string UserId { get; set; } // Submission can have One User
        public User User { get; set; } // Navigation prop
    }
}