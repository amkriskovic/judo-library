using System.Collections.Generic;
using JudoLibrary.Models.Abstractions;

namespace JudoLibrary.Models
{
    public class Category : SlugModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public IList<SubCategory> SubCategories { get; set; } // One Category can have Many SubCategories 
    }
}