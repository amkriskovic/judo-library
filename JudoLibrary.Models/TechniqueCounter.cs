namespace JudoLibrary.Models
{
    public class TechniqueCounter
    {
        public int TechniqueId { get; set; }
        public Technique Technique { get; set; }
        public int CounterId { get; set; }
        public Technique Counter { get; set; } // One Technique can have Many Counters
        public bool Active { get; set; } 
    }
}