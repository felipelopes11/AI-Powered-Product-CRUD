using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CadastroProdutosAI.Core.Models;
using CadastroProdutosAI.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CadastroProdutosAI.UI.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly ProdutoService _produtoService = new();
    
    public ObservableCollection<Produto> Produtos { get; } = new();

    [ObservableProperty]
    private string _nomeProduto = "";

    [ObservableProperty]
    private string _precoProduto = "";
    
    [ObservableProperty]
    private string _estoqueProduto = "";

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AtualizarProdutoCommand))]
    [NotifyCanExecuteChangedFor(nameof(RemoverProdutoCommand))]
    private Produto? _produtoSelecionado;
    
    public MainWindowViewModel()
    {
        CarregarProdutos();
    }

    private async Task CarregarProdutos()
    {
        Produtos.Clear();
        var listaDeProdutos = await _produtoService.ObterTodosProdutos();
        foreach (var produto in listaDeProdutos)
        {
            Produtos.Add(produto);
        }
    }
    
    [RelayCommand]
    private async Task AdicionarProduto()
    {
        // CORREÇÃO: Usamos as propriedades públicas (NomeProduto) em vez dos campos privados (_nomeProduto)
        var novoProduto = new Produto
        {
            Nome = NomeProduto,
            Preco = decimal.TryParse(PrecoProduto, out var preco) ? preco : 0,
            Estoque = int.TryParse(EstoqueProduto, out var estoque) ? estoque : 0
        };

        await _produtoService.AdicionarProduto(novoProduto);
        LimparCampos();
        await CarregarProdutos();
    }

    [RelayCommand(CanExecute = nameof(PodeModificar))]
    private async Task AtualizarProduto()
    {
        // CORREÇÃO: Usamos a propriedade pública (ProdutoSelecionado) em vez do campo privado (_produtoSelecionado)
        if (ProdutoSelecionado is null) return;
        
        var produtoAtualizado = new Produto
        {
            Id = ProdutoSelecionado.Id,
            Nome = NomeProduto,
            Preco = decimal.TryParse(PrecoProduto, out var preco) ? preco : 0,
            Estoque = int.TryParse(EstoqueProduto, out var estoque) ? estoque : 0
        };
        
        await _produtoService.AtualizarProduto(produtoAtualizado);
        LimparCampos();
        await CarregarProdutos();
    }
    
    [RelayCommand(CanExecute = nameof(PodeModificar))]
    private async Task RemoverProduto()
    {
        // CORREÇÃO: Usamos a propriedade pública (ProdutoSelecionado) em vez do campo privado (_produtoSelecionado)
        if (ProdutoSelecionado is null) return;

        await _produtoService.RemoverProduto(ProdutoSelecionado.Id);
        LimparCampos();
        await CarregarProdutos();
    }
    
    private bool PodeModificar()
    {
        // CORREÇÃO: Usamos a propriedade pública (ProdutoSelecionado) em vez do campo privado (_produtoSelecionado)
        return ProdutoSelecionado is not null;
    }

    partial void OnProdutoSelecionadoChanged(Produto? value)
    {
        if (value is not null)
        {
            NomeProduto = value.Nome;
            PrecoProduto = value.Preco.ToString("F2");
            EstoqueProduto = value.Estoque.ToString();
        }
    }

    private void LimparCampos()
    {
        NomeProduto = "";
        PrecoProduto = "";
        EstoqueProduto = "";
        ProdutoSelecionado = null;
    }
}