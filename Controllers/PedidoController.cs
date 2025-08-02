using Carregamentos.Models;
using Carregamentos.Services;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace Carregamentos.Controllers
{
    public class PedidoController : Controller
    {
        private readonly ApiDataService _apiDataService;

        public PedidoController(ApiDataService apiDataService)
        {
            _apiDataService = apiDataService;
        }

        // Endpoint para consumo via Ajax (retorna JSON)
        // Renomeado 'destino' para 'filtro' para maior consistência
        [HttpGet]
        public async Task<IActionResult> GetPedidos(string? filtro) // Usar string? para permitir nulo
        {
            var pedidos = await _apiDataService.GetPedidosAsync(filtro);
            return Json(pedidos);
        }

        // View inicial que carrega página com dados e layout
        public async Task<IActionResult> Index(string? filtro)
        {
            var pedidos = await _apiDataService.GetPedidosAsync(filtro);
            var rotas = await _apiDataService.GetRotasAgrupadasAsync();

            var viewModel = new CarregamentoViewModel
            {
                Pedidos = pedidos,
                RotasAgrupadas = rotas,
                NumPedFilter = filtro,
                TotalVlAtend = pedidos.Sum(p => p.VlAtend ?? 0)
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Detalhes(string numped)
        {
            var itens = await _apiDataService.GetItensPedidoAsync(numped);

            var pedido = new Pedido
            {
                Numped = numped,
                ItensDoPedido = itens
            };

            return View(pedido);
        }
    }
}