using System.Collections.Generic;
using System.Linq;
using System.Threading.Channels;
using System.Threading.Tasks;
using JudoLibrary.Api.BackgroundServices;
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
        [HttpPost]
        // Injecting channel of type EditVideoMessage
        public async Task<Submission> CreateSubmission(
            [FromBody] Submission submission,
            [FromServices] Channel<EditVideoMessage> channel)
        {
            // Marking video as NOT processed
            submission.VideoProcessed = false;
            
            _context.Add(submission);
            await _context.SaveChangesAsync();
            
            // Grab the channel writer, and write an item to a channel with SubmissionId & Input props
            await channel.Writer.WriteAsync(new EditVideoMessage
            {
                // Save submissionId for particular upload
                SubmissionId = submission.Id,
                
                // Input is original video -> string
                Input = submission.Video
            });

            return submission;
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