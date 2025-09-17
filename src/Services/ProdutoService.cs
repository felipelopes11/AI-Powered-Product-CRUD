using System.Collections.Generic;
using System.Threading.Tasks;
using CadastroProdutosAI.Core.Models;
using CadastroProdutosAI.Data.Repositories;

namespace CadastroProdutosAI.Services;

public class ProdutoService
{
    private readonly ProdutoRepository _produtoRepository = new();

    public Task<List<Produto>> ObterTodosProdutos()
    {
        return _produtoRepository.ListarTodos();
    }
    
    // NOVO:
    public Task AdicionarProduto(Produto produto)
    {
        return _produtoRepository.Adicionar(produto);
    }
    
    // NOVO:
    public Task AtualizarProduto(Produto produto)
    {
        return _produtoRepository.Atualizar(produto);
    }

    // NOVO:
    public Task RemoverProduto(int id)
    {
        return _produtoRepository.Remover(id);
    }
}