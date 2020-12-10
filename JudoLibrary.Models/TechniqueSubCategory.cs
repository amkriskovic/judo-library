namespace JudoLibrary.Models
{
    public class TechniqueSubCategory
    {
        public int TechniqueId { get; set; }
        public Technique Technique { get; set; }

        public int SubCategoryId { get; set; }
        public SubCategory SubCategory { get; set; }
        public bool Active { get; set; } 
    }
}