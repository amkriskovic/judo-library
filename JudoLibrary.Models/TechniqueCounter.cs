namespace JudoLibrary.Models
{
    public class TechniqueCounter
    {
        public string TechniqueId { get; set; }
        public Technique Technique { get; set; } 

        public string CounterId { get; set; }
        public Technique Counter { get; set; } // One Technique can have Many Counters
    }
}