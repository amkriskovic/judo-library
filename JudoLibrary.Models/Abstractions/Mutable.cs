using System;

namespace JudoLibrary.Models.Abstractions
{
    // Vote belongs to User
    public class Mutable : BaseModel<int>
    {
        public DateTime Updated { get; set; } = DateTime.UtcNow;

        public string UserId { get; set; } 
        public User User { get; set; }  
    }
}