namespace CosmosDBExampleInNeo4j
{
    using System.Threading.Tasks;

    internal class Program
    {
        static async Task Main(string[] args)
        {
            CosmosDBVersion.Run();
            await Neo4jCloseConversion.Run();
        }
    }
}