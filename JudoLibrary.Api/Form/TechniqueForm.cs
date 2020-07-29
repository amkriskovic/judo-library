namespace JudoLibrary.Api.Form
{
    public class TechniqueForm
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string CategoryId { get; set; } // One TechniqueForm can have One Category
        public string SubCategoryId { get; set; } // One TechniqueForm can have One SubCategory
    }
}