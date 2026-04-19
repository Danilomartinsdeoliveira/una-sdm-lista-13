using AmericanAirlinesApi.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AmericanAirlinesApi.Controllers
{
    [ApiController]
    [Route("api/radar")]
    public class FlightRadarController : ControllerBase
    {
        private readonly AppDbContext _db;
        public FlightRadarController(AppDbContext db) => _db = db;

        /// <summary>
        /// Retorna quantos voos ativos estão indo para cada destino.
        /// Simula latência de consulta a serviço externo de satélite.
        /// GET api/radar/proximos-destinos
        /// </summary>
        [HttpGet("proximos-destinos")]
        public async Task<IActionResult> ProximosDestinos()
        {
            // Simula consulta a serviço externo de satélite (latência de 2 segundos)
            Console.WriteLine("📡 Consultando satélite... aguardando resposta...");
            Thread.Sleep(2000);

            var voosAtivos = await _db.Voos
                .Where(v => v.Status == "Agendado" || v.Status == "Em Voo")
                .GroupBy(v => v.Destino)
                .Select(g => new
                {
                    Destino = g.Key,
                    TotalVoos = g.Count(),
                    VoosEmTransito = g.Count(v => v.Status == "Em Voo"),
                    VoosAgendados = g.Count(v => v.Status == "Agendado")
                })
                .OrderByDescending(x => x.TotalVoos)
                .ToListAsync();

            return Ok(new
            {
                geradoEm = DateTime.UtcNow,
                fonteSimulada = "SatelliteTrackingService_v3",
                latenciaSimuladaMs = 2000,
                totalDestinos = voosAtivos.Count,
                destinos = voosAtivos
            });
        }

        /// <summary>
        /// Resumo geral do radar: todos os voos agrupados por status.
        /// GET api/radar/status-geral
        /// </summary>
        [HttpGet("status-geral")]
        public async Task<IActionResult> StatusGeral()
        {
            var resumo = await _db.Voos
                .GroupBy(v => v.Status)
                .Select(g => new { Status = g.Key, Total = g.Count() })
                .ToListAsync();

            return Ok(new
            {
                geradoEm = DateTime.UtcNow,
                resumoPorStatus = resumo
            });
        }
    }
}
