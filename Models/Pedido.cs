using Newtonsoft.Json;
using System.Collections.Generic;

namespace Carregamentos.Models
{
    public class Pedido
    {
        [JsonProperty("NUMPED")]
        public string? Numped { get; set; }

        [JsonProperty("CODCLI")]
        public string? CodCli { get; set; }

        [JsonProperty("CLIENTE")]
        public string? Cliente { get; set; }

        [JsonProperty("DATA_PEDIDO")]
        public string? DataPedido { get; set; }

        [JsonProperty("DATA_LIBERACAO")]
        public string? DataLiberacao { get; set; }

        [JsonProperty("DATA_MONTAGEM")]
        public string? DataMontagem { get; set; }

        [JsonProperty("DESTINO")]
        public string? Destino { get; set; }

        [JsonProperty("NOMECIDADE")]
        public string? NomeCidade { get; set; }

        [JsonProperty("STATUS_CARREGAMENTO")]
        public string? StatusCarregamento { get; set; }

        [JsonProperty("VLATEND")]
        public decimal? VlAtend { get; set; }

        [JsonProperty("QT_ENTREGAS")]
        public int? QEntregas { get; set; }

        [JsonProperty("QT_NOTAS")]
        public int? QNotas { get; set; }

        [JsonProperty("ROTA")]
        public string? Rota { get; set; }

        [JsonProperty("BAIRRO")]
        public string? Bairro { get; set; }

        [JsonProperty("CODUSUR")]
        public string? CodUsur { get; set; }

        [JsonProperty("RCA")]
        public string? Rca { get; set; }

        [JsonProperty("CODPRACA")]
        public string? CodPraca { get; set; }

        [JsonProperty("PRACA")]
        public string? Praca { get; set; }

        [JsonProperty("POSICAO")]
        public string? Posicao { get; set; }

        [JsonProperty("TIPO_PEDIDO")]
        public string? TipoPedido { get; set; }

        [JsonProperty("UF")]
        public string? Uf { get; set; }

        [JsonProperty("NUMCAR")]
        public string? NumCar { get; set; }

        [JsonProperty("itens_do_pedido")]
        public List<ItemPedido>? ItensDoPedido { get; set; }
    }
}
