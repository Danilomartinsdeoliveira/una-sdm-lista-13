using AmericanAirlinesApi.Data;
using AmericanAirlinesApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AmericanAirlinesApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VooController : ControllerBase
    {
        private readonly AppDbContext _db;
        public VooController(AppDbContext db) => _db = db;

        // GET api/voo
        [HttpGet]
        public async Task<IActionResult> GetAll()
            => Ok(await _db.Voos.Include(v => v.Aeronave).Include(v => v.Reservas).ToListAsync());

        // GET api/voo/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var voo = await _db.Voos
                .Include(v => v.Aeronave)
                .Include(v => v.Reservas)
                .FirstOrDefaultAsync(v => v.Id == id);
            return voo is null ? NotFound() : Ok(voo);
        }

        // POST api/voo
        // Regra A: Agendamento Inteligente
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Voo voo)
        {
            // Verificar se a aeronave existe
            var aeronave = await _db.Aeronaves.FindAsync(voo.AeronaveId);
            if (aeronave is null)
                return NotFound(new { mensagem = $"Aeronave com Id {voo.AeronaveId} não encontrada." });

            // Verificar se a aeronave está em voo
            bool aeronaveEmTransito = await _db.Voos
                .AnyAsync(v => v.AeronaveId == voo.AeronaveId && v.Status == "Em Voo");

            if (aeronaveEmTransito)
                return Conflict(new { mensagem = "Aeronave indisponível, encontra-se em trânsito." });

            voo.Status = "Agendado";
            _db.Voos.Add(voo);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = voo.Id }, voo);
        }

        // PATCH api/voo/{id}/status
        // Regra C: Atualização de Status com Regra de Ouro
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> AtualizarStatus(int id, [FromBody] AtualizarStatusDto dto)
        {
            var voo = await _db.Voos.FindAsync(id);
            if (voo is null) return NotFound();

            var statusFinais = new[] { "Finalizado", "Cancelado" };
            bool statusAtualEFinal = statusFinais.Contains(voo.Status);
            bool tentaVoltar = dto.NovoStatus == "Em Voo";

            // Regra de Ouro: voo Finalizado ou Cancelado não pode voltar para Em Voo
            if (statusAtualEFinal && tentaVoltar)
                return UnprocessableEntity(new
                {
                    mensagem = $"Operação inválida: voo com status '{voo.Status}' não pode voltar para 'Em Voo'."
                });

            voo.Status = dto.NovoStatus;
            await _db.SaveChangesAsync();
            return Ok(new { mensagem = $"Status do voo {voo.CodigoVoo} atualizado para '{voo.Status}'.", voo });
        }

        // DELETE api/voo/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var voo = await _db.Voos.FindAsync(id);
            if (voo is null) return NotFound();
            _db.Voos.Remove(voo);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }

    public record AtualizarStatusDto(string NovoStatus);
}
