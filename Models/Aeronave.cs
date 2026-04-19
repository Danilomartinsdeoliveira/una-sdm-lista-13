namespace AmericanAirlinesApi.Models
{
    public class Aeronave
    {
        public int Id { get; set; }
        public string Modelo { get; set; } = string.Empty;
        public string CodigoCauda { get; set; } = string.Empty;
        public int CapacidadePassageiros { get; set; }

        // Navigation
        public ICollection<Voo> Voos { get; set; } = new List<Voo>();
    }
}
