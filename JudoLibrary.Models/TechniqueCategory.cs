namespace JudoLibrary.Models
{
    public class TechniqueCategory
    {
        public int TechniqueId { get; set; }
        public Technique Technique { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public bool Active { get; set; } 
    }
}