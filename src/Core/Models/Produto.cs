namespace CadastroProdutosAI.Core.Models;

/// <summary>
/// Representa a entidade Produto.
/// Esta classe é o "molde" para um produto no nosso sistema.
/// </summary>
public class Produto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty; // Valor padrão para evitar nulos
    public decimal Preco { get; set; }
    public int Estoque { get; set; }
}