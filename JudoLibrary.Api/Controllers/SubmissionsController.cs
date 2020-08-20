using System.Collections.Generic;
using System.Linq;
using System.Threading.Channels;
using System.Threading.Tasks;
using JudoLibrary.Api.BackgroundServices;
using JudoLibrary.Api.BackgroundServices.VideoEditing;
using JudoLibrary.Api.Form;
using JudoLibrary.Data;
using JudoLibrary.Models;
using Microsoft.AspNetCore.Mvc;

namespace JudoLibrary.Api.Controllers
{
    [ApiController]
    [Route("api/submissions")]
    public class SubmissionsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SubmissionsController(AppDbContext context)
        {
            _context = context;
        }
        
        // GET -> /api/submissions
        [HttpGet]
        public IEnumerable<Submission> GetAllSubmissions() => _context.Submissions
            .Where(s => s.VideoProcessed)
            .ToList();

        // GET -> /api/submissions/{id}
        [HttpGet("{id}")]
        public Submission GetSubmission(int id) => _context.Submissions.FirstOrDefault(s => s.Id.Equals(id));

        // POST -> /api/submissions
        // [FromServices] =>> Dependency Injection as method parameter
        [HttpPost]
        public async Task<IActionResult> CreateSubmission(
            [FromBody] SubmissionForm submissionForm,
            [FromServices] Channel<EditVideoMessage> channel,
            [FromServices] VideoManager videoManager)
        {
            // Validate video path
            // If temporary video does NOT exist based on provided video name
            if (!videoManager.TemporaryFileExists(submissionForm.Video))
            {
                // Return bad request, we cant convert -> we dont have video -> hasn't been uploaded/stored successfully
                return BadRequest();
            }
            
            // Create new Submission
            var submission = new Submission
            {
                // Grab the input fields from submission form / content creation
                TechniqueId = submissionForm.TechniqueId,
                Description = submissionForm.Description,
                
                // Marking video as NOT processed when we create new Submission
                VideoProcessed = false
            };
            
            // Add created submission to DB
            _context.Add(submission);
            
            // Save changes to DB
            await _context.SaveChangesAsync();
            
            // * Channel producer
            // * We dont write to a channel if form wasn't successful submitted
            // Grab the channel writer, and write an item/msg(EditVideoMessage) to a channel with SubmissionId & Input props
            await channel.Writer.WriteAsync(new EditVideoMessage
            {
                // Save submissionId for particular upload
                SubmissionId = submission.Id,
                
                // Input is original video -> string that comes from input when selecting video from submission form
                Input = submissionForm.Video
            });

            // Return newly created submission
            return Ok(submission);
        }
        
        // PUT -> /api/submissions
        [HttpPut]
        public async Task<Submission> UpdateSubmission([FromBody] Submission submission)
        {
            // Check if submission exists
            if (submission.Id == 0)
                return null;
            
            _context.Add(submission);
            await _context.SaveChangesAsync();

            return submission;
        }

        // DELETE -> /api/submissions/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSubmission(int id)
        {
            // Grab submission from DB
            var submission = _context.Submissions.FirstOrDefault(s => s.Id.Equals(id));

            // Check if submission is null
            if (submission == null)
                return NotFound();
            
            // Mark submission as deleted
            submission.Deleted = true;

            await _context.SaveChangesAsync();
            
            return Ok();
        }
    }
}