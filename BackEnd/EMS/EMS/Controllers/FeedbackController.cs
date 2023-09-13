using EMS.Context;
using EMS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EMS.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class FeedbackController : Controller
    {
        private readonly AddDbContext _addDbContext;
        public FeedbackController(AddDbContext addDbContext)
        {
            _addDbContext = addDbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllFeedbacks()
        {
            var feedbacks = await _addDbContext.Feedbacks.ToListAsync();

            return Ok(feedbacks);
        }

        [HttpPost]
        public async Task<IActionResult> GiveFeedback([FromBody] Feedback feedback)
        {
            feedback.Id = Guid.NewGuid();

            await _addDbContext.Feedbacks.AddAsync(feedback);
            await _addDbContext.SaveChangesAsync();

            return Ok(feedback);
        }

        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetFeedback([FromRoute] Guid id)
        {
            var feedback = await _addDbContext.Feedbacks.FirstOrDefaultAsync(x => x.Id == id);

            if (feedback == null)
            {
                return NotFound();
            }
            return Ok(feedback);
        }

        [HttpDelete]
        [Route("{id:Guid}")]
        public async Task<IActionResult> DeleteFeedback([FromRoute] Guid id)
        {
            var feedback = await _addDbContext.Feedbacks.FindAsync(id);

            if (feedback == null)
            {
                return NotFound();
            }
            _addDbContext.Feedbacks.Remove(feedback);
            await _addDbContext.SaveChangesAsync();
            return Ok(feedback);
        }
    }
    }

