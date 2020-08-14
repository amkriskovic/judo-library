using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;

namespace JudoLibrary.Api.BackgroundServices.VideoEditing
{
    public class VideoManager
    {
        // Provides information about the web hosting environment that application is running in.
        private readonly IWebHostEnvironment _env;
        private const string TempPrefix = "temp_";
        private const string ConvertedPrefix = "conv_";

        public VideoManager(IWebHostEnvironment env)
        {
            _env = env;
        }
        
        // Points to working directory
        public string WorkingDirectory => _env.WebRootPath;
        
        // Returns string -> temp save path for video
        public string TemporarySavePath(string fileName)
        {
            // Combines path for saving video to wwwroot folder with video name
            return Path.Combine(WorkingDirectory, fileName);
        }
        
        // Returns bool -> checks if video is temporary
        public bool IsTemporary(string fileName)
        {
            // Return true if video name starts with "temp_"
            return fileName.StartsWith(TempPrefix);
        }
        
        // Returns bool -> checks if temporary video exists in working dir path
        public bool TemporaryVideoExists(string fileName)
        {
            // Create temporary save path based on file name
            var path = TemporarySavePath(fileName);

            // Return true if video is in provided path
            return File.Exists(path);
        }
        
        // Deletes temporary video in path based on provided video name
        public void DeleteTemporaryVideoInPath(string fileName)
        {
            // Create temporary save path based on file name
            var path = TemporarySavePath(fileName);
            
            // Delete video from path
            File.Delete(path);
        }
        
        // Returns string -> path to video based on provided video name
        public string DevVideoPath(string fileName)
        {
            // If env is NOT dev, return null otherwise ...
            // Combines web root path (wwwroot) + fileName which is name of video
            return !_env.IsDevelopment() ? null : TemporarySavePath(fileName) ;
        }

        // Returns string -> converted video file name
        public string GenerateConvertedFileName() => $"{ConvertedPrefix}{DateTime.Now.Ticks}.mp4";
        
        // Returns string -> constructing name for saving temp video
        // IFormFile =>> Represents a file sent with the HttpRequest.
        public async Task<string> SaveTemporaryVideo(IFormFile video)
        {
            // Returns string which we combine with temp prefix, unique ticks which are based on time and
            // getting extension from video file name, example: .mp4, .mpeg
            var fileName = string.Concat(TempPrefix, DateTime.Now.Ticks, Path.GetExtension(video.FileName));
            
            // Create temp save path based on video name
            var savePath = TemporarySavePath(fileName);
            
            // Use File Stream where we provide save path <- where to save video, create, and write
            // FileMode.Create specifies that the operating system should create a new file
            // FileAccess.Write specifies that data can be written to the file
            await using (var fileStream = new FileStream(savePath, FileMode.Create, FileAccess.Write))
            {
                // Provide the stream that we want to put it in
                // Asynchronously reads the bytes from the current stream and writes them to another stream
                await video.CopyToAsync(fileStream);
            }
            
            // Returns temporary video name
            return fileName;
        }
    }
}