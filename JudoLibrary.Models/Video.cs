namespace JudoLibrary.Models
{
    public class Video : BaseModel<int>
    {
        public string ThumbLink { get; set; }
        public string VideoLink { get; set; }
    }
}