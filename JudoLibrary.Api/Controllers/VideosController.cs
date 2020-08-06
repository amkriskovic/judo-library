using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JudoLibrary.Api.Controllers
{
    [Route("api/videos")]
    public class VideosController : ControllerBase
    {
        // Provides information about the web hosting environment that application is running in
        private readonly IWebHostEnvironment _environment;

        public VideosController(IWebHostEnvironment environment)
        {
            _environment = environment;
        }
        
        // GET -> /api/videos/{video}
        // Action for getting video based on his name
        [HttpGet("{video}")]
        public IActionResult GetVideo(string video)
        {
            // Grab the mime from video string (e.g. mp4, mpeg)
            var mime = video.Split('.').Last();
            
            // Save path for uploaded videos
            var savePath = Path.Combine(_environment.WebRootPath, video);
            
            // Use file stream where we provide save path, open file and read file (video)
            // Returns file stream result combining file stream and content type, which give us ability to play
            return new FileStreamResult(new FileStream(savePath, FileMode.Open, FileAccess.Read), "video/*");
        }
        
        // POST -> /api/videos
        // IFormFile must be of name video, because it's named in pages form name="video"
        // video binds to form where: name="video"
        public async Task<IActionResult> UploadVideo(IFormFile video)
        {
            // Grab the type of video (extension e.g. mp4, mpeg)
            var mime = video.FileName.Split('.').Last();
            
            // Creating <- constructing file name | . | appending mime
            // GetRandomFileName generates random string for file name
            var fileName = string.Concat(Path.GetRandomFileName(), ".", mime);
            
            // Creating path for uploaded videos
            // Combines path for saving video to wwwroot folder with random file name
            var savePath = Path.Combine(_environment.WebRootPath, fileName);
            
            // Use File Stream where we provide save path <- where to save video, create, and write
            // FileMode.Create specifies that the operating system should create a new file
            // FileAccess.Write specifies that data can be written to the file

            await using (var fileStream = new FileStream(savePath, FileMode.Create, FileAccess.Write))
            {
                // Provide the stream that we want to put it in
                // Asynchronously reads the bytes from the current stream and writes them to another stream
                await video.CopyToAsync(fileStream);
            }

            
            // Queues the specified work to run on the thread pool and returns a Task object that represents that work.
            // * Creating Task to run and awaiting for result from it => running in background => not blocking Thread
            await Task.Run(() =>
            {
                // Object for starting process,
                // Specifies a set of values that are used when you start a process.
                var startInfo = new ProcessStartInfo
                {
                    // Gets or sets the application or document to start.
                    // Getting ffmpeg executable which provides CLI tools
                    FileName = Path.Combine(_environment.ContentRootPath, "ffmpeg", "ffmpeg.exe"),
                    
                    // Gets or sets the set of command-line arguments to use when starting the application.
                    // Specifying arguments for manipulating video, savePath -> video location with name -> original + transcoded
                    Arguments = $"-y -i {savePath} -an -vf scale=640x480 test.mp4",
                    
                    // Gets or sets the working directory for the process to be started
                    // Specifying where things are happening
                    WorkingDirectory = _environment.WebRootPath,

                    // Gets or sets a value indicating whether to start the process in a new window.
                    CreateNoWindow = true,

                    // Gets or sets a value indicating whether to use the operating 
                    // system shell to start the process
                    UseShellExecute = false
                };

                // Provides access to local and remote processes and enables you to start and stop local system processes.
                // Creating new Process object and assigning startInfo information's to prop of Process class
                using var process = new Process {StartInfo = startInfo};
                
                // Start the process
                process.Start();

                // Wait for exit
                process.WaitForExit();
            });

            return Ok(fileName);
        }
    }
}