using System.Collections.Generic;
using JudoLibrary.Models.Abstractions;

namespace JudoLibrary.Models
{
    public class SubCategory : VersionedModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string CategoryId { get; set; } // One SubCategory can have One Category
        public IList<TechniqueSubCategory> Techniques { get; set; } = new List<TechniqueSubCategory>(); // One SubCategory can have many Techniques 
    }
}