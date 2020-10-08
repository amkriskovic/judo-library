namespace JudoLibrary.Api.Settings
{
    public class FileSettings
    {
        public string VideoUrl { get; set; }
        public string ImageUrl { get; set; }
        public string Provider { get; set; } // Local or S3 Storage
    }
}