namespace AmericanAirlinesApi.Models
{
    public class Tripulante
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;

        /// <summary>Piloto | Copiloto | Comissário</summary>
        public string Funcao { get; set; } = string.Empty;
        public string NumeroLicenca { get; set; } = string.Empty;
    }
}
