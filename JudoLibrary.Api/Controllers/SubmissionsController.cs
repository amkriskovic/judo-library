using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        public IEnumerable<Submission> GetAllSubmissions() => _context.Submissions.ToList();

        // GET -> /api/submissions/{id}
        [HttpGet("{id}")]
        public Submission GetSubmission(int id) => _context.Submissions.FirstOrDefault(s => s.Id.Equals(id));

        // POST -> /api/submissions
        [HttpPost]
        public async Task<Submission> CreateSubmission([FromBody] Submission submission)
        {
            _context.Add(submission);
            await _context.SaveChangesAsync();

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