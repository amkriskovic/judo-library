using System;
using System.IO;
using System.Threading.Tasks;
using JudoLibrary.Api.Settings;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace JudoLibrary.Api.BackgroundServices.VideoEditing
{
    public class FileManagerLocal : IFileManager
    {
        // Provides information about the web hosting environment that application is running in.
        private readonly IWebHostEnvironment _env;
        private readonly IOptionsMonitor<FileSettings> _fileSettingsMonitor;


        // Injecting FileSettings with IOptionsMonitor => DI, when service get updated/resolved we get new value 
        // Depends/Pulls on appsettings.dev.json FileSettings
        public FileManagerLocal(IWebHostEnvironment env, IOptionsMonitor<FileSettings> fileSettingsMonitor)
        {
            _env = env;
            _fileSettingsMonitor = fileSettingsMonitor;
        }

        // Proxy for TempPrefix
        private static string TempPrefix => JudoLibraryConstants.Files.TempPrefix;
        
        // Points to working directory
        private string WorkingDirectory => _env.WebRootPath;

        // Points to FFMPEG executable
        public string GetFFMPEGPath() => Path.Combine(_env.ContentRootPath, "ffmpeg", "ffmpeg.exe");
        
        // Method for resolving file URL
        public string GetFileUrl(string fileName, FileType fileType)
        {
            // Getting current value out of _fileSettingsMonitor -> re-reading the file settings
            var settings = _fileSettingsMonitor.CurrentValue;
            
            // Switch file type
            return fileType switch
            {
                // If it's image
                FileType.Image =>
                    // Return image url (path) / filename
                    $"{settings.ImageUrl}/{fileName}",
                
                // If it's video ...
                FileType.Video => $"{settings.VideoUrl}/{fileName}",
                
                // Default -> invalid file type
                _ => throw new ArgumentOutOfRangeException(nameof(fileType), fileType, null)
            };
        }

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

        // Returns bool -> checks if temporary file exists in working dir path
        public bool TemporaryFileExists(string fileName)
        {
            // Created temporary save path based on file name
            var path = TemporarySavePath(fileName);

            // Return true if file is in provided path
            return File.Exists(path);
        }

        // Deletes temporary file in path based on provided file name
        public void DeleteTemporaryFileInPath(string fileName)
        {
            // Created temporary save path based on file name
            var path = TemporarySavePath(fileName);

            // If file exists in path
            if (File.Exists(path))
            {
                // Delete file from path
                File.Delete(path);
            }
        }

        // Returns string -> path to video based on provided video name
        public string GetSavePath(string fileName)
        {
            // Combines web root path (wwwroot) + fileName which is name of video
            return TemporarySavePath(fileName);
        }

        // Returns string -> constructing name for saving temp video
        // IFormFile =>> Represents a file sent with the HttpRequest.
        public async Task<string> SaveTemporaryFile(IFormFile video)
        {
            // Returns string which we combine with temp prefix, unique ticks which are based on time and
            // getting extension from video file name, example: .mp4, .mpeg
            var fileName = string.Concat(TempPrefix, DateTime.Now.Ticks, Path.GetExtension(video.FileName));

            // Created temp save path based on video name
            var savePath = TemporarySavePath(fileName);

            // Use File Stream where we provide save path <- where to save video, create, and write
            // FileMode.Created specifies that the operating system should create a new file
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