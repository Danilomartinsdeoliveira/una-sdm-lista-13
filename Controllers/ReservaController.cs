using AmericanAirlinesApi.Data;
using AmericanAirlinesApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AmericanAirlinesApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReservaController : ControllerBase
    {
        private readonly AppDbContext _db;
        public ReservaController(AppDbContext db) => _db = db;

        // GET api/reserva
        [HttpGet]
        public async Task<IActionResult> GetAll()
            => Ok(await _db.Reservas.Include(r => r.Voo).ThenInclude(v => v!.Aeronave).ToListAsync());

        // GET api/reserva/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var reserva = await _db.Reservas
                .Include(r => r.Voo).ThenInclude(v => v!.Aeronave)
                .FirstOrDefaultAsync(r => r.Id == id);
            return reserva is null ? NotFound() : Ok(reserva);
        }

        // POST api/reserva
        // Regra B: Check-in — Overbooking + Assento Janela
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Reserva reserva)
        {
            // Busca o voo com aeronave e reservas já existentes
            var voo = await _db.Voos
                .Include(v => v.Aeronave)
                .Include(v => v.Reservas)
                .FirstOrDefaultAsync(v => v.Id == reserva.VooId);

            if (voo is null)
                return NotFound(new { mensagem = $"Voo com Id {reserva.VooId} não encontrado." });

            // Validação de Overbooking
            int totalReservas = voo.Reservas.Count;
            int capacidade = voo.Aeronave!.CapacidadePassageiros;

            if (totalReservas >= capacidade)
                return BadRequest(new { mensagem = "Voo lotado. Não é possível realizar novas reservas." });

            // Lógica de Assento Janela (termina com A ou F)
            decimal valorBase = 200.00m;
            char ultimaLetraAssento = reserva.Assento.ToUpper().Last();

            if (ultimaLetraAssento == 'A' || ultimaLetraAssento == 'F')
            {
                valorBase += 50.00m;
                Console.WriteLine("✈️  Assento na janela reservado com sucesso!");
            }

            reserva.Valor = valorBase;
            _db.Reservas.Add(reserva);
            await _db.SaveChangesAsync();

            // Recarrega com navegação para retornar dados completos
            await _db.Entry(reserva).Reference(r => r.Voo).LoadAsync();

            return CreatedAtAction(nameof(GetById), new { id = reserva.Id }, new
            {
                reserva.Id,
                reserva.NomePassageiro,
                reserva.Assento,
                reserva.Valor,
                assentoJanela = ultimaLetraAssento == 'A' || ultimaLetraAssento == 'F',
                voo = new { voo.Id, voo.CodigoVoo, voo.Origem, voo.Destino, voo.Status },
                aeronave = new { voo.Aeronave!.Modelo, voo.Aeronave.CodigoCauda },
                ocupacao = $"{totalReservas + 1}/{capacidade}"
            });
        }

        // DELETE api/reserva/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var reserva = await _db.Reservas.FindAsync(id);
            if (reserva is null) return NotFound();
            _db.Reservas.Remove(reserva);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
