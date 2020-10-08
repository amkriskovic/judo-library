using System;
using System.IO;
using System.Threading.Tasks;
using JudoLibrary.Api.BackgroundServices.VideoEditing;
using JudoLibrary.Api.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JudoLibrary.Api.Controllers
{
    [Route("api/files")]
    public class FileController : ControllerBase
    {
        private readonly IFileManager _fileManagerLocal;

        public FileController(IFileManager fileManagerLocal)
        {
            _fileManagerLocal = fileManagerLocal;
        }
        
        // GET -> /api/files/{type}/{file}
        // Action for getting file type based on his name
        [HttpGet("{type}/{file}")]
        public IActionResult GetVideo(string type, string file)
        {
            // Switch the "type" to extract mime from it based on FileType
            // If it's image -> image/jpg | If it's video -> video/mp4
            var mime = type.Equals(nameof(FileType.Image), StringComparison.InvariantCultureIgnoreCase)
                ? "image/jpg"
                : type.Equals(nameof(FileType.Video), StringComparison.InvariantCultureIgnoreCase)
                    ? "video/mp4"
                    : null;

            // If mime is null -> 400 you are requesting invalid type
            if (mime == null) return BadRequest();
                
            // Create saving path for uploaded file
            var savePath = _fileManagerLocal.TemporarySavePath(file);
            
            // If savePath path is null or empty
            if (string.IsNullOrEmpty(savePath))
            {
                // Return bad request -> 404
                return BadRequest();
            }
            
            // Use file stream where we provide save path, open file and read file (file)
            // Returns file stream result combining file stream and content type, which will be resolved in image or video format
            return new FileStreamResult(new FileStream(savePath, FileMode.Open, FileAccess.Read), mime);
        }
        
        // POST -> /api/files
        // IFormFile must be of name video, because it's named in pages form name="video"
        // file binds to form where: name="video"
        [HttpPost]
        public Task<string> UploadVideo(IFormFile video)
        {
            // Saves temporary file based on file that is passed from form
            return _fileManagerLocal.SaveTemporaryFile(video);
        }
        
        // DELETE -> /api/files/{fileName}
        // Action for deleting temporary file by passing fileName param which comes from frontend.
        // It can be deleted after submission is created (save) or 
        // when someone uploads file but cancels the form in the middle or right after uploading
        [HttpDelete("{fileName}")]
        public IActionResult DeleteTemporaryVideo(string fileName)
        {
            // If file name/file name, is not temporary based on file name
            if (!_fileManagerLocal.IsTemporary(fileName))
            {
                // Return bad request, means it's not temporary, we dont wanna delete it -> 400
                return BadRequest();
            }
            
            // If temporary file does NOT exist based on file name
            if (!_fileManagerLocal.TemporaryFileExists(fileName))
            {
                // Return no content -> 404
                return NoContent();
            }
            
            // If it does exists, call DeleteTemporaryFileInPath method which accepts file name and deletes based on that
            _fileManagerLocal.DeleteTemporaryFileInPath(fileName);

            // Return Ok -> It has been deleted
            return Ok();
        }
    }
}