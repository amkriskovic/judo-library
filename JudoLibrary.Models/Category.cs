using System.Collections.Generic;

namespace JudoLibrary.Models
{
    public class Category : BaseModel<string>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public IList<SubCategory> SubCategories { get; set; } // One Category can have Many SubCategories 
    }
}