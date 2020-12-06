using System.Collections.Generic;
using JudoLibrary.Models.Abstractions;

namespace JudoLibrary.Models
{
    public class Category : Mutable<string>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        // public IList<SubCategory> SubCategories { get; set; } // One Category can have Many SubCategories 
        public IList<Technique> Techniques { get; set; } // One SubCategory can have many Techniques 
    }
}