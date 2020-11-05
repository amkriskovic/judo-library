using System;

namespace JudoLibrary.Models.Abstractions
{
    public abstract class BaseModel<TKey>
    {
        public TKey Id { get; set; }
        public bool Deleted { get; set; }
        
        public DateTime Created { get; set; } = DateTime.UtcNow;
    }
}