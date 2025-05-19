using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Lab8Csharp.Models;

namespace InotMppAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProbaController : ControllerBase
    {
        private readonly InotContextE _context;
        private readonly WebSocketHandler _wsService;

        public ProbaController(InotContextE context, WebSocketHandler wsService)
        {
            _context = context;
            _wsService = wsService;
        }

        // GET: api/proba
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProbaE>>> GetAllProbe()
        {
            var probe = await _context.Probas.ToListAsync();
            return Ok(probe);
        }

        // GET: api/proba/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProbaE>> GetProba(int id)
        {
            var proba = await _context.Probas.FindAsync(id);

            if (proba == null)
                return NotFound();

            return Ok(proba);
        }

        // POST: api/proba
        [HttpPost]
        public async Task<ActionResult<ProbaE>> AddProba(ProbaE proba)
        {
            _context.Probas.Add(proba);
            await _context.SaveChangesAsync();

            await _wsService.BroadcastAsync($"Proba {proba.Id} added");

            return CreatedAtAction(nameof(GetProba), new { id = proba.Id }, proba);
        }

        // PUT: api/proba/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProba(int id, ProbaE proba)
        {
            if (id != proba.Id)
                return BadRequest();

            _context.Entry(proba).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                await _wsService.BroadcastAsync($"Proba {id} updated");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Probas.AnyAsync(e => e.Id == id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // DELETE: api/proba/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProba(int id)
        {
            var proba = await _context.Probas.FindAsync(id);
            if (proba == null)
                return NotFound();

            _context.Probas.Remove(proba);
            await _context.SaveChangesAsync();

            await _wsService.BroadcastAsync($"Proba {id} deleted");

            return NoContent();
        }
    }
}
