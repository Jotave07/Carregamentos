using Carregamentos.Models;
using Carregamentos.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace Carregamentos.Controllers
{
    public class HomeController : Controller
    {
        private readonly OracleDataService _oracleDataService;

        public HomeController(OracleDataService oracleDataService)
        {
            _oracleDataService = oracleDataService;
        }

        public async Task<IActionResult> Index(string? numPedFilter)
        {
            ViewData["FilterValue"] = numPedFilter ?? "";

            var rotasAgrupadas = await _oracleDataService.GetRotasAgrupadasAsync();
            var pedidos = await _oracleDataService.GetPedidosAsync(numPedFilter);

            var model = GetModel(numPedFilter, rotasAgrupadas, pedidos);

            model.TotalVlAtend = model.Pedidos.Sum(p => p.VlAtend ?? 0);

            decimal valorFreteCalculado = 0;
            if (Request.Method == "POST" && decimal.TryParse(Request.Form["valorFrete"], NumberStyles.Currency, CultureInfo.GetCultureInfo("pt-BR"), out decimal parsedFrete))
            {
                valorFreteCalculado = parsedFrete;
            }

            model.ValorFrete = valorFreteCalculado;

            model.ValorPagarRota = model.TotalVlAtend * 0.05m;

            if (model.TotalVlAtend > 0)
            {
                model.PorcentagemFrete = (model.ValorFrete / model.TotalVlAtend) * 100;
            }
            else
            {
                model.PorcentagemFrete = 0;
            }

            model.RotaSelecionada = "Nenhuma rota selecionada";

            return View(model);
        }

        private static CarregamentoViewModel GetModel(string? numPedFilter, Dictionary<string, decimal> rotasAgrupadas, List<Pedido> pedidos)
        {
            return new CarregamentoViewModel
            {
                RotasAgrupadas = rotasAgrupadas,
                Pedidos = pedidos,
                NumPedFilter = numPedFilter
            };
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        public async Task<IActionResult> GetItensPedido(string numPed)
        {
            if (string.IsNullOrEmpty(numPed))
            {
                return BadRequest("Número do pedido é obrigatório.");
            }

            var itens = await _oracleDataService.GetItensPedidoAsync(numPed);

            if (itens == null || !itens.Any())
            {
                return Json(new List<ItemPedido>()); // Retorna array vazio para evitar erros JS
            }

            return Json(itens);
        }
    }
}
