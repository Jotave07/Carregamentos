using Carregamentos.Models;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System;

namespace Carregamentos.Services
{
    public class OracleDataService
    {
        private readonly string _connectionString;

        public OracleDataService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("OracleConnection")
                                ?? throw new ArgumentNullException(nameof(configuration), "OracleConnection string cannot be null.");
        }

        public async Task<List<Pedido>> GetPedidosAsync(string? filter = null)
        {
            var pedidos = new List<Pedido>();
            using (var connection = new OracleConnection(_connectionString))
            {
                string query = @"
                    SELECT NUMPED, CODCLI, CLIENTE, DATA_PEDIDO, DATA_LIBERACAO,
                           DATA_MONTAGEM, DESTINO, NOMECIDADE, VLATEND, STATUS_CARREGAMENTO,
                           QT_ENTREGAS, QT_NOTAS, ROTA, BAIRRO, CODUSUR, RCA,
                           CODPRACA, PRACA, POSICAO, TIPO_PEDIDO, UF, NUMCAR
                    FROM PEDIDOS
                    WHERE 1=1";

                if (!string.IsNullOrEmpty(filter))
                {
                    // Busca exata, use LIKE para busca parcial
                    query += " AND NUMPED LIKE '%' || :numPedFilter || '%'";
                }

                using (var command = new OracleCommand(query, connection))
                {
                    if (!string.IsNullOrEmpty(filter))
                    {
                        command.Parameters.Add(new OracleParameter("numPedFilter", filter));
                    }

                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            pedidos.Add(new Pedido
                            {
                                Numped = reader["NUMPED"].ToString(),
                                CodCli = reader["CODCLI"].ToString(),
                                Cliente = reader["CLIENTE"].ToString(),
                                DataPedido = reader["DATA_PEDIDO"].ToString(),
                                DataLiberacao = reader["DATA_LIBERACAO"].ToString(),
                                DataMontagem = reader["DATA_MONTAGEM"].ToString(),
                                Destino = reader["DESTINO"].ToString(),
                                NomeCidade = reader["NOMECIDADE"].ToString(),

                                VlAtend = reader.IsDBNull(reader.GetOrdinal("VLATEND")) ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal("VLATEND")),

                                StatusCarregamento = reader["STATUS_CARREGAMENTO"].ToString(),

                                QEntregas = reader.IsDBNull(reader.GetOrdinal("QT_ENTREGAS")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("QT_ENTREGAS")),
                                QNotas = reader.IsDBNull(reader.GetOrdinal("QT_NOTAS")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("QT_NOTAS")),

                                Rota = reader["ROTA"].ToString(),
                                Bairro = reader["BAIRRO"].ToString(),
                                CodUsur = reader["CODUSUR"].ToString(),
                                Rca = reader["RCA"].ToString(),
                                CodPraca = reader["CODPRACA"].ToString(),
                                Praca = reader["PRACA"].ToString(),
                                Posicao = reader["POSICAO"].ToString(),
                                TipoPedido = reader["TIPO_PEDIDO"].ToString(),
                                Uf = reader["UF"].ToString(),
                                NumCar = reader["NUMCAR"].ToString()
                            });
                        }
                    }
                }
            }
            return pedidos;
        }

        public async Task<Dictionary<string, decimal>> GetRotasAgrupadasAsync()
        {
            var rotas = new Dictionary<string, decimal>();
            using (var connection = new OracleConnection(_connectionString))
            {
                string query = "SELECT ROTA, SUM(VLATEND) AS TOTAL_VL_ATEND FROM PEDIDOS GROUP BY ROTA";
                using (var command = new OracleCommand(query, connection))
                {
                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            string rota = reader["ROTA"].ToString() ?? "Desconhecido";
                            decimal total = reader.IsDBNull(reader.GetOrdinal("TOTAL_VL_ATEND")) ? 0 : reader.GetDecimal(reader.GetOrdinal("TOTAL_VL_ATEND"));
                            rotas.Add(rota, total);
                        }
                    }
                }
            }
            return rotas;
        }

        public async Task<List<ItemPedido>> GetItensPedidoAsync(string numPed)
        {
            var itens = new List<ItemPedido>();
            using (var connection = new OracleConnection(_connectionString))
            {
                string query = @"
                    SELECT CODPROD, DESCRICAO, QT, PVENDA, SUBTOTAL
                    FROM ITENS_PEDIDO
                    WHERE NUMPED = :numPed";
                using (var command = new OracleCommand(query, connection))
                {
                    command.Parameters.Add(new OracleParameter("numPed", numPed));
                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            itens.Add(new ItemPedido
                            {
                                CODPROD = reader["CODPROD"].ToString(),
                                DESCRICAO = reader["DESCRICAO"].ToString(),
                                QT = reader.IsDBNull(reader.GetOrdinal("QT")) ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal("QT")),
                                PVENDA = reader.IsDBNull(reader.GetOrdinal("PVENDA")) ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal("PVENDA")),
                                SUBTOTAL = reader.IsDBNull(reader.GetOrdinal("SUBTOTAL")) ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal("SUBTOTAL"))
                            });
                        }
                    }
                }
            }
            return itens;
        }
    }
}
