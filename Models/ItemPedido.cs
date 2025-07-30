using Newtonsoft.Json;

namespace Carregamentos.Models
{
    public class ItemPedido
    {
        [JsonProperty("CODPROD")]
        public string? CODPROD { get; set; }

        [JsonProperty("DESCRICAO")]
        public string? DESCRICAO { get; set; }

        [JsonProperty("QT")]
        public decimal? QT { get; set; }

        [JsonProperty("PVENDA")]
        public decimal? PVENDA { get; set; }

        [JsonProperty("SUBTOTAL")]
        public decimal? SUBTOTAL { get; set; }
    }
}
