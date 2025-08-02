using Carregamentos.Models;
using Carregamentos.Services;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace Carregamentos.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApiDataService _apiDataService;

        public HomeController(ApiDataService apiDataService)
        {
            _apiDataService = apiDataService;
        }

        public async Task<IActionResult> Index(string? filtro = null)
        {
            var model = new CarregamentoViewModel
            {
                NumPedFilter = filtro
            };

            // Carrega pedidos conforme filtro, ou todos se null
            var pedidos = await _apiDataService.GetPedidosAsync(filtro);
            model.Pedidos = pedidos;

            // Carrega rotas agrupadas
            model.RotasAgrupadas = await _apiDataService.GetRotasAgrupadasAsync();

            // Se passou filtro e tem ao menos um pedido, define RotaSelecionada
            model.RotaSelecionada = filtro != null && pedidos.Any() ? pedidos.First().Destino : null;

            // Calcula total atendido
            model.TotalVlAtend = pedidos.Sum(p => p.VlAtend ?? 0);

            return View(model);
        }
    }
}
