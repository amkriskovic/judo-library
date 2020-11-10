namespace JudoLibrary.Models.Abstractions
{
    // Vote belongs to User
    public class Vote : BaseModel<int>
    {
        public string UserId { get; set; } 
        public User User { get; set; }  
    }
}