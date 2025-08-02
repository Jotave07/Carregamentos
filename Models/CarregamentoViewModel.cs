using System.Collections.Generic;

namespace Carregamentos.Models
{
    public class CarregamentoViewModel
    {
        public Dictionary<string, decimal> RotasAgrupadas { get; set; } = new();
        public List<Pedido> Pedidos { get; set; } = new();
        public string? NumPedFilter { get; set; }

        public decimal TotalVlAtend { get; set; }
        public decimal ValorFrete { get; set; }
        public decimal ValorPagarRota { get; set; }
        public decimal PorcentagemFrete { get; set; }
        public string? RotaSelecionada { get; set; }
    }
}
