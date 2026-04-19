using AmericanAirlinesApi.Data;
using AmericanAirlinesApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AmericanAirlinesApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AeronaveController : ControllerBase
    {
        private readonly AppDbContext _db;
        public AeronaveController(AppDbContext db) => _db = db;

        
        [HttpGet]
        public async Task<IActionResult> GetAll()
            => Ok(await _db.Aeronaves.ToListAsync());

        
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var aeronave = await _db.Aeronaves.FindAsync(id);
            return aeronave is null ? NotFound() : Ok(aeronave);
        }

        
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Aeronave aeronave)
        {
            _db.Aeronaves.Add(aeronave);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = aeronave.Id }, aeronave);
        }

        // PUT api/aeronave/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Aeronave updated)
        {
            var aeronave = await _db.Aeronaves.FindAsync(id);
            if (aeronave is null) return NotFound();

            aeronave.Modelo = updated.Modelo;
            aeronave.CodigoCauda = updated.CodigoCauda;
            aeronave.CapacidadePassageiros = updated.CapacidadePassageiros;

            await _db.SaveChangesAsync();
            return Ok(aeronave);
        }

        // DELETE api/aeronave/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var aeronave = await _db.Aeronaves.FindAsync(id);
            if (aeronave is null) return NotFound();
            _db.Aeronaves.Remove(aeronave);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
