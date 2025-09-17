using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CadastroProdutosAI.Services;

public record GeminiPart([property: JsonPropertyName("text")] string Text);
public record GeminiContent([property: JsonPropertyName("parts")] GeminiPart[] Parts);
public record SafetySetting([property: JsonPropertyName("category")] string Category, [property: JsonPropertyName("threshold")] string Threshold);
public record GeminiRequest([property: JsonPropertyName("contents")] GeminiContent[] Contents, [property: JsonPropertyName("safetySettings")] SafetySetting[] SafetySettings);

public class ChatAiService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _systemPrompt;
    private readonly string _geminiUrl;

    public ChatAiService(string apiKey)
    {
        _apiKey = apiKey;
        _httpClient = new HttpClient();
        _geminiUrl = "https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash-latest:generateContent";
        _httpClient.DefaultRequestHeaders.Add("x-goog-api-key", _apiKey);

        // ===== PROMPT ATUALIZADO COM CAPACIDADE DE ALTERAÇÃO =====
        _systemPrompt = @"
            Você é um assistente de gerenciamento de produtos. Sua tarefa é interpretar o comando do usuário e traduzi-lo para uma das funções disponíveis. Seja amigável.
            Sua resposta DEVE SER SEMPRE um objeto JSON válido, contido dentro de um bloco ```json ... ```, e nada mais.

            As funções disponíveis são:
            1. `listar_produtos`: Retorna a lista de todos os produtos.
            2. `adicionar_produto`: Requer `nome` (string), `preco` (number), `estoque` (number).
            3. `atualizar_produto`: Requer um identificador (`id` ou `nome`) e pelo menos um campo para alterar (`nome`, `preco`, ou `estoque`).
            4. `remover_produto`: Requer `id` (number).
            5. `consultar_produto`: Requer `id` (number) ou `nome` (string).
            6. `conversa_simples`: Para cumprimentos ou perguntas gerais.
            7. `erro`: Se não entender o comando.

            Exemplos:
            - Usuário: 'liste os itens' -> ```json{""funcao"": ""listar_produtos"", ""parametros"": {}}```
            - Usuário: 'adicione teclado por 150 reais com 10 unidades' -> ```json{""funcao"": ""adicionar_produto"", ""parametros"": {""nome"": ""teclado"", ""preco"": 150.0, ""estoque"": 10}}```
            - Usuário: 'Altere o preço do teclado gamer para 250' -> ```json{""funcao"": ""atualizar_produto"", ""parametros"": {""nome"": ""teclado gamer"", ""preco"": 250.0}}```
            - Usuário: 'mude o estoque do item 1 para 99' -> ```json{""funcao"": ""atualizar_produto"", ""parametros"": {""id"": 1, ""estoque"": 99}}```
            - Usuário: 'renomeie o produto 5 para Teclado Gamer PRO' -> ```json{""funcao"": ""atualizar_produto"", ""parametros"": {""id"": 5, ""nome"": ""Teclado Gamer PRO""}}```
            - Usuário: 'qual o preço do mouse gamer?' -> ```json{""funcao"": ""consultar_produto"", ""parametros"": {""nome"": ""mouse gamer""}}```
            - Usuário: 'oi, tudo bem?' -> ```json{""funcao"": ""conversa_simples"", ""parametros"": {""resposta"": ""Olá! Sou seu assistente de produtos. Como posso ajudar?""}}```
        ";
    }

    public async Task<JsonElement> ExecutarComando(string comandoUsuario)
    {
        // O resto da classe continua igual
        var promptCompleto = $"{_systemPrompt}\n\nComando do usuário: '{comandoUsuario}'";
        
        var requestBody = new GeminiRequest(
            Contents: new[] { new GeminiContent(new[] { new GeminiPart(promptCompleto) }) },
            SafetySettings: new[]
            {
                new SafetySetting("HARM_CATEGORY_HARASSMENT", "BLOCK_NONE"),
                new SafetySetting("HARM_CATEGORY_HATE_SPEECH", "BLOCK_NONE"),
                new SafetySetting("HARM_CATEGORY_SEXUALLY_EXPLICIT", "BLOCK_NONE"),
                new SafetySetting("HARM_CATEGORY_DANGEROUS_CONTENT", "BLOCK_NONE")
            }
        );

        try
        {
            var response = await _httpClient.PostAsJsonAsync(_geminiUrl, requestBody);
            response.EnsureSuccessStatusCode(); 

            var responseBody = await response.Content.ReadFromJsonAsync<JsonElement>();
            
            var aiTextResponse = responseBody
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text").GetString() ?? "";

            var jsonResponse = aiTextResponse
                .Replace("```json", "")
                .Replace("```", "")
                .Trim();
                
            var jsonDoc = JsonDocument.Parse(jsonResponse);
            return jsonDoc.RootElement;
        }
        catch (Exception ex)
        {
            string erroReal = $"Erro na API: {ex.Message}";
            var erroPayload = new { funcao = "erro", parametros = new { mensagem = erroReal } };
            var erroJson = JsonDocument.Parse(JsonSerializer.Serialize(erroPayload));
            return erroJson.RootElement;
        }
    }
}