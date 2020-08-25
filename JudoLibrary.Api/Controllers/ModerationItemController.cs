using System.Collections.Generic;
using System.Linq;
using JudoLibrary.Data;
using JudoLibrary.Models.Moderation;
using Microsoft.AspNetCore.Mvc;

namespace JudoLibrary.Api.Controllers
{
    [ApiController]
    [Route("/api/moderation-items")]
    public class ModerationItemController : ControllerBase
    {
        private readonly AppDbContext _ctx;

        public ModerationItemController(AppDbContext ctx)
        {
            _ctx = ctx;
        }
        
        // GET -> /api/moderation-items
        [HttpGet]
        public IEnumerable<ModerationItem> GetModerationItems() => _ctx.ModerationItems.ToList();
        
        // GET -> /api/moderation-items/{id}
        [HttpGet("{id}")]
        public ModerationItem GetModerationItem(int id) => _ctx.ModerationItems.FirstOrDefault(mi => mi.Id.Equals(id));
    }
}