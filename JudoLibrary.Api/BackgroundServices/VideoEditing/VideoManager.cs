﻿using System;
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
        private const string ThumbnailPrefix = "thumb_";
        private const string ProfilePrefix = "profile_";

        public VideoManager(IWebHostEnvironment env)
        {
            _env = env;
        }

        // Points to working directory
        private string WorkingDirectory => _env.WebRootPath;

        // Points to FFMPEG executable
        public string FFMPEGPath => Path.Combine(_env.ContentRootPath, "ffmpeg", "ffmpeg.exe");

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
            // Create temporary save path based on file name
            var path = TemporarySavePath(fileName);

            // Return true if file is in provided path
            return File.Exists(path);
        }

        // Deletes temporary file in path based on provided file name
        public void DeleteTemporaryFileInPath(string fileName)
        {
            // Create temporary save path based on file name
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
            // If env is NOT dev, return null otherwise ...
            // Combines web root path (wwwroot) + fileName which is name of video
            return !_env.IsDevelopment() ? null : TemporarySavePath(fileName);
        }

        // Returns string -> converted video file name => e.g. .mp4
        public static string GenerateConvertedFileName() => $"{ConvertedPrefix}{DateTime.Now.Ticks}.mp4";

        // Returns string -> thumbnail for video file name => e.g. .png
        public static string GenerateThumbnailFileName() => $"{ThumbnailPrefix}{DateTime.Now.Ticks}.jpg";
        
        // Returns string -> thumbnail for profile picture file name => e.g. .png
        public static string GenerateProfileFileName() => $"{ProfilePrefix}{DateTime.Now.Ticks}.jpg";

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