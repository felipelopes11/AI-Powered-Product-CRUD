using CadastroProdutosAI.Web.Components;
using CadastroProdutosAI.Services; // Importa o namespace dos nossos serviços

var builder = WebApplication.CreateBuilder(args);

// Adiciona os serviços padrão do Blazor ao container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();


// ===== CONFIGURAÇÃO DOS NOSSOS SERVIÇOS CUSTOMIZADOS =====

// 1. Registra o serviço que gerencia os produtos.
//    'AddScoped' significa que uma nova instância será criada para cada requisição do usuário.
builder.Services.AddScoped<ProdutoService>();

// 2. Registra o serviço da IA.
//    Primeiro, lemos a chave de API do arquivo de configuração.
var geminiApiKey = builder.Configuration["GeminiApiKey"];

//    Em seguida, fazemos uma verificação de segurança para garantir que a chave foi configurada.
if (string.IsNullOrEmpty(geminiApiKey) || geminiApiKey.Contains("COLE_SUA_CHAVE_AQUI"))
{
    // Se a chave não estiver correta, a aplicação vai parar aqui com uma mensagem de erro clara.
    throw new InvalidOperationException("A chave de API do Gemini não foi configurada corretamente em appsettings.Development.json");
}
//    'Singleton' significa que uma única instância será criada e usada por toda a aplicação.
builder.Services.AddSingleton(new ChatAiService(geminiApiKey));

// ===== FIM DA CONFIGURAÇÃO DOS SERVIÇOS =====


var app = builder.Build();

// Configura o pipeline de requisições HTTP (como o servidor se comporta).
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();