using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JudoLibrary.Api.Controllers
{
    [ApiController]
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

            return Ok(fileName);
        }
    }
}