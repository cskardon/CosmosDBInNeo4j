namespace CosmosDBExampleInNeo4j
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Neo4j.Driver;
    using Newtonsoft.Json;

    public class Neo4jCloseConversion
    {
        private const string Hostname = "localhost";
        private const int Port = 7687;
        private const string Username = "neo4j";
        private const string Password = "neo";
        private const string Database = "neo4j";

        private static Dictionary<string, string> CypherQueries { get; } = new Dictionary<string, string>
        {
            {"Cleanup", "MATCH (n) DETACH DELETE n;"},
            {"AddVertex 1", "CREATE (:Person {id: 'thomas', firstName: 'Thomas', age: 44});"},
            {"AddVertex 2", "CREATE (:Person {id: 'mary', firstName: 'Mary', lastName: 'Andersen', age: 39});"},
            {"AddVertex 3", "CREATE (:Person {id: 'ben', firstName: 'Ben', lastName: 'Miller'});"},
            {"AddVertex 4", "CREATE (:Person {id: 'robin', firstName: 'Robin', lastName: 'Wakefield'});"},
            {"AddEdge 1", "MATCH (thomas:Person {id: 'thomas'}), (mary:Person {id: 'mary'}) CREATE (thomas)-[:KNOWS]->(mary);"},
            {"AddEdge 2", "MATCH (thomas:Person {id: 'thomas'}), (ben:Person {id: 'ben'}) CREATE (thomas)-[:KNOWS]->(ben);"},
            {"AddEdge 3", "MATCH (ben:Person {id: 'ben'}), (robin:Person {id: 'robin'}) CREATE (ben)-[:KNOWS]->(robin);"},
            {"UpdateVertex", "MATCH (thomas:Person {id: 'thomas'}) SET thomas.age = 44;"},
            {"CountVertices", "MATCH (n) RETURN count(n);"},
            {"Filter Range", "MATCH (x:Person) WHERE x.age > 40 RETURN x;"},
            {"Project", "MATCH (x:Person) RETURN x.firstName;"},
            {"Sort", "MATCH (x:Person) RETURN x ORDER BY x.firstName DESC;"},
            {"Traverse", "MATCH (:Person {id: 'thomas'})-[:KNOWS]->(known:Person) RETURN known; //???"},
            {"Traverse 2x", "MATCH (:Person {id: 'thomas'})-[:KNOWS]->(:Person)-[:KNOWS]->(knownKnows:Person) RETURN knownKnows; "},
            {"Loop", "MATCH path=(:Person {id: 'thomas'})-[*]->(:Person {id:'robin'}) RETURN path;"},
            {"DropEdge", "MATCH (:Person {id: 'thomas'})-[k:KNOWS]->(:Person {id:'mary'}) DELETE k;"},
            {"CountEdges", "MATCH ()-[r]->() RETURN count(r);"},
            {"DropVertex", "MATCH (thomas:Person {id: 'thomas'}) DETACH DELETE thomas;"}
        };

        // Starts a console application that executes every Cypher query in the CypherQueries dictionary. 
        public static async Task Run()
        {
            using var driver = GraphDatabase.Driver($"neo4j://{Hostname}:{Port}", AuthTokens.Basic(Username, Password));
            {
                var session = driver.AsyncSession(config => config.WithDatabase(Database));
                foreach (var cypherQuery in CypherQueries)
                {
                    Console.WriteLine();
                    Console.WriteLine($"Running this query: {cypherQuery.Key} {cypherQuery.Value}");
                    var results = await ExecuteQuery(session, cypherQuery.Value);
                    Console.WriteLine("\tResult");
                    foreach (var result in await results.ToListAsync())
                    {
                        var output = JsonConvert.SerializeObject(result);
                        Console.WriteLine($"\t{output}");
                    }

                    Console.WriteLine(JsonConvert.SerializeObject(await results.ConsumeAsync()));
                }

                await session.CloseAsync();
            }
        }

        private static Task<IResultCursor> ExecuteQuery(IAsyncSession session, string cypher)
        {
            try
            {
                return session.RunAsync(cypher);
            }
            catch (Neo4jException ex)
            {
                Console.WriteLine("\tError!");
                Console.WriteLine($"\t{ex}");
                throw;
            }
        }
    }
}