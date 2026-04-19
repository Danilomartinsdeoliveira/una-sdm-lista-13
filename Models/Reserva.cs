namespace AmericanAirlinesApi.Models
{
    public class Reserva
    {
        public int Id { get; set; }

        public int VooId { get; set; }
        public Voo? Voo { get; set; }

        public string NomePassageiro { get; set; } = string.Empty;

        /// <summary>Ex: "12A", "14F"</summary>
        public string Assento { get; set; } = string.Empty;

        /// <summary>Valor calculado: $50 de taxa se assento for janela (A ou F). Valor base: $200.</summary>
        public decimal Valor { get; set; }

        public DateTime DataReserva { get; set; } = DateTime.UtcNow;
    }
}
