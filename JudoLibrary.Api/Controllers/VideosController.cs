using System.IO;
using System.Threading.Tasks;
using JudoLibrary.Api.BackgroundServices.VideoEditing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JudoLibrary.Api.Controllers
{
    [Route("api/videos")]
    public class VideosController : ControllerBase
    {
        private readonly VideoManager _videoManager;

        public VideosController(VideoManager videoManager)
        {
            _videoManager = videoManager;
        }
        
        // GET -> /api/videos/{video}
        // Action for getting video based on his name
        [HttpGet("{video}")]
        public IActionResult GetVideo(string video)
        {
            // Create saving path for uploaded video
            var savePath = _videoManager.TemporarySavePath(video);
            
            // If savePath path is null or empty
            if (string.IsNullOrEmpty(savePath))
            {
                // Return bad request -> 404
                return BadRequest();
            }
            
            // Use file stream where we provide save path, open file and read file (video)
            // Returns file stream result combining file stream and content type, which give us ability to play
            return new FileStreamResult(new FileStream(savePath, FileMode.Open, FileAccess.Read), "video/*");
        }
        
        // POST -> /api/videos
        // IFormFile must be of name video, because it's named in pages form name="video"
        // video binds to form where: name="video"
        [HttpPost]
        public Task<string> UploadVideo(IFormFile video)
        {
            // Saves temporary video based on video that is passed from form
            return _videoManager.SaveTemporaryVideo(video);
        }
        
        // DELETE -> /api/videos/{fileName}
        // Action for deleting temporary video by passing fileName param which comes from frontend.
        // It can be deleted after submission is created (save) or 
        // when someone uploads video but cancels the form in the middle or right after uploading
        [HttpDelete("{fileName}")]
        public IActionResult DeleteTemporaryVideo(string fileName)
        {
            // If file name/video name, is not temporary based on file name
            if (!_videoManager.IsTemporary(fileName))
            {
                // Return bad request, means it's not temporary, we dont wanna delete it -> 400
                return BadRequest();
            }
            
            // If temporary video does NOT exist based on file name
            if (!_videoManager.TemporaryVideoExists(fileName))
            {
                // Return no content -> 404
                return NoContent();
            }
            
            // If it does exists, call DeleteTemporaryVideoInPath method which accepts file name and deletes based on that
            _videoManager.DeleteTemporaryVideoInPath(fileName);

            // Return Ok -> It has been deleted
            return Ok();
        }
    }
}