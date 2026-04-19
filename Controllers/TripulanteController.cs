using AmericanAirlinesApi.Data;
using AmericanAirlinesApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AmericanAirlinesApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TripulanteController : ControllerBase
    {
        private readonly AppDbContext _db;
        public TripulanteController(AppDbContext db) => _db = db;

        // GET api/tripulante
        [HttpGet]
        public async Task<IActionResult> GetAll()
            => Ok(await _db.Tripulantes.ToListAsync());

        // GET api/tripulante/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var t = await _db.Tripulantes.FindAsync(id);
            return t is null ? NotFound() : Ok(t);
        }

        // POST api/tripulante
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Tripulante tripulante)
        {
            _db.Tripulantes.Add(tripulante);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = tripulante.Id }, tripulante);
        }

        // PUT api/tripulante/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Tripulante updated)
        {
            var t = await _db.Tripulantes.FindAsync(id);
            if (t is null) return NotFound();

            t.Nome = updated.Nome;
            t.Funcao = updated.Funcao;
            t.NumeroLicenca = updated.NumeroLicenca;

            await _db.SaveChangesAsync();
            return Ok(t);
        }

        // DELETE api/tripulante/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var t = await _db.Tripulantes.FindAsync(id);
            if (t is null) return NotFound();
            _db.Tripulantes.Remove(t);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
