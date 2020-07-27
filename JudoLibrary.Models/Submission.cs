namespace JudoLibrary.Models
{
    public class Submission : BaseModel<int>
    {
        public string Video { get; set; }
        public string Description { get; set; }
        public string TechniqueId { get; set; } // Submission can have One Technique 
    }
}