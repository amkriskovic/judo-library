namespace JudoLibrary.Models
{
    public class Submission : BaseModel<int>
    {
        public int VideoId { get; set; }
        public Video Video { get; set; } // Submission can have One Video 
        public string Description { get; set; }
        public bool VideoProcessed { get; set; }
        public string TechniqueId { get; set; } // Submission can have One Technique 
    }
}