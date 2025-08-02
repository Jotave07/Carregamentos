using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Carregamentos.Models;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using System.Linq;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;

namespace Carregamentos.Services
{
    public sealed class ApiDataService
    {
        private readonly HttpClient _httpClient;

        public ApiDataService(HttpClient httpClient, IConfiguration config)
        {
            var baseUrl = config["ApiSettings:FlaskApiBaseUrl"];
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(baseUrl ?? throw new ArgumentNullException(nameof(baseUrl)));
        }

        /// <summary>
        /// Obtém a lista de pedidos do endpoint da API Flask.
        /// </summary>
        /// <param name="filtro">Filtro opcional para busca.</param>
        /// <param name="cancellationToken">Token de cancelamento.</param>
        /// <returns>Lista de pedidos.</returns>
        public async Task<List<Pedido>> GetPedidosAsync(string? filtro = null, CancellationToken cancellationToken = default)
        {
            try
            {
                string endpoint = "/fretes";
                var response = await _httpClient.GetAsync(endpoint, cancellationToken);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var pedidos = JsonConvert.DeserializeObject<List<Pedido>>(content) ?? new();

                if (!string.IsNullOrEmpty(filtro))
                {
                    var filtroNormalizado = filtro.Trim();
                    if (filtroNormalizado.All(char.IsDigit))
                    {
                        // Filtro por número do pedido (ignora zeros à esquerda)
                        var filtroSemZeros = filtroNormalizado.TrimStart('0');
                        pedidos = pedidos
                            .Where(p => (p.Numped ?? "").TrimStart('0') == filtroSemZeros)
                                .ToList();
                    }
                    else
                    {
                        // Filtro por destino (rota)
                        pedidos = pedidos
                            .Where(p => string.Equals(p.Destino?.Trim(), filtroNormalizado, StringComparison.OrdinalIgnoreCase))
                            .ToList();
                    }
                }

                return pedidos;
            }
            catch (HttpRequestException)
            {
                return new();
            }
            catch (System.Text.Json.JsonException)
            {
                return new();
            }
        }

        public async Task<List<ItemPedido>> GetItensPedidoAsync(string numped)
        {
            var response = await _httpClient.GetAsync($"/fretes/numped?numped={Uri.EscapeDataString(numped)}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<ItemPedido>>(content) ?? new();
        }

        // Corrigido: o endpoint retorna List<Pedido>, que será agrupado por rota.
        public async Task<Dictionary<string, decimal>> GetRotasAgrupadasAsync()
        {
            var response = await _httpClient.GetAsync("/fretes");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var pedidos = JsonConvert.DeserializeObject<List<Pedido>>(content) ?? new();

            // Agrupa os pedidos por Destino e soma o VlAtend, depois ordena do maior para o menor valor
            return pedidos
                .GroupBy(p => p.Destino)
                .Select(g => new { Destino = g.Key, Total = g.Sum(p => p.VlAtend ?? 0) })
                .OrderByDescending(x => x.Total)
                .ToDictionary(x => x.Destino, x => x.Total);
        }
    }
}
