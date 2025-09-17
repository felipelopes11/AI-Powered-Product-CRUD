using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CadastroProdutosAI.Core.Models;
using MySql.Data.MySqlClient;

namespace CadastroProdutosAI.Data.Repositories;

public class ProdutoRepository
{
    private static readonly string _connectionString = Connection.GetConnectionString();

    // MÉTODO DE LEITURA (JÁ EXISTENTE)
    public async Task<List<Produto>> ListarTodos()
    {
        var produtos = new List<Produto>();
        using (var connection = new MySqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            var query = "SELECT Id, Nome, Preco, Estoque FROM produtos;";
            using (var command = new MySqlCommand(query, connection))
            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    produtos.Add(new Produto
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        Nome = Convert.ToString(reader["Nome"]),
                        Preco = Convert.ToDecimal(reader["Preco"]),
                        Estoque = Convert.ToInt32(reader["Estoque"])
                    });
                }
            }
        }
        return produtos;
    }

    // NOVO: MÉTODO PARA ADICIONAR PRODUTO
    public async Task Adicionar(Produto produto)
    {
        using (var connection = new MySqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            var query = "INSERT INTO produtos (Nome, Preco, Estoque) VALUES (@Nome, @Preco, @Estoque);";
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Nome", produto.Nome);
                command.Parameters.AddWithValue("@Preco", produto.Preco);
                command.Parameters.AddWithValue("@Estoque", produto.Estoque);
                await command.ExecuteNonQueryAsync();
            }
        }
    }

    // NOVO: MÉTODO PARA ATUALIZAR PRODUTO
    public async Task Atualizar(Produto produto)
    {
        using (var connection = new MySqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            var query = "UPDATE produtos SET Nome = @Nome, Preco = @Preco, Estoque = @Estoque WHERE Id = @Id;";
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Nome", produto.Nome);
                command.Parameters.AddWithValue("@Preco", produto.Preco);
                command.Parameters.AddWithValue("@Estoque", produto.Estoque);
                command.Parameters.AddWithValue("@Id", produto.Id);
                await command.ExecuteNonQueryAsync();
            }
        }
    }

    // NOVO: MÉTODO PARA REMOVER PRODUTO
    public async Task Remover(int id)
    {
        using (var connection = new MySqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            var query = "DELETE FROM produtos WHERE Id = @Id;";
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Id", id);
                await command.ExecuteNonQueryAsync();
            }
        }
    }
}