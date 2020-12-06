using System.Collections.Generic;
using JudoLibrary.Models.Abstractions;

namespace JudoLibrary.Models
{
    public class SubCategory : Mutable<string>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string CategoryId { get; set; } // One SubCategory can have One Category
        public IList<Technique> Techniques { get; set; } // One SubCategory can have many Techniques 
    }
}