namespace CadastroProdutosAI.Data;

internal static class Connection
{
    // IMPORTANTE: Substitua com os dados da sua instalação do MySQL
    // A senha é aquela que você definiu ao instalar o MySQL Server.
    public static string GetConnectionString()
    {
        const string server = "localhost";
        const string database = "crud_ai";
        const string user = "root";
        const string password = "123456"; // <-- TROQUE AQUI

        return $"Server={server};Database={database};Uid={user};Pwd={password};";
    }
}