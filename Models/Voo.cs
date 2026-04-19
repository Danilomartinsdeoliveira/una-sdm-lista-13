namespace AmericanAirlinesApi.Models
{
    public class Voo
    {
        public int Id { get; set; }
        public string CodigoVoo { get; set; } = string.Empty;
        public string Origem { get; set; } = string.Empty;
        public string Destino { get; set; } = string.Empty;

        public int AeronaveId { get; set; }
        public Aeronave? Aeronave { get; set; }

        public DateTime DataHoraPartida { get; set; }
        public DateTime DataHoraChegada { get; set; }

        /// <summary>Agendado | Em Voo | Finalizado | Cancelado</summary>
        public string Status { get; set; } = "Agendado";

        // Navigation
        public ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
    }
}
