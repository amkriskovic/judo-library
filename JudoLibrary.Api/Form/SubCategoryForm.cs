namespace JudoLibrary.Api.Form
{
    public class SubCategoryForm
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string CategoryId { get; set; } // One SubCategory can have One Category
    }
}