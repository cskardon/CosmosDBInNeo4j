# CosmosDB Example In Neo4j

If you go to the Azure [Cosmos DB Introduction Page](https://docs.microsoft.com/en-us/azure/cosmos-db/introduction) you can sign up for a free trial, and as part of that you get an example .NET project to play with.

I though it might be interesting to take that standard example template and write it for Neo4j.

## What do you need?

To run the CosmosDB code - you'll need a CosmosDB instance, to run the Neo4j examples - you'll want Neo4j 4.x - easily available to run locally via [Neo4j Desktop](https://neo4j.com/download/), or try it on the cloud at [Neo4j Aura](https://neo4j.com/aura/).

## How do I run it?

```
dotnet build
dotnet run
```

## Files

* `Program.cs`
  * This just runs the examples, feel free to comment out the ones you don't want to run.
* `CosmosDBVersion.cs`
  * This is what you get from the CosmosDB example codebase
* `Neo4jCloseConversion.cs`
  * This is as close as I can make a Neo4j one to the above. 

## Notes

_Closeness_ - As I'm basing it off of the CosmosDB example, there are things they do that I wouldn't do - and the output's are different, but the queries all do the same as each other.

_Query Translation_ - OK, technically, they aren't _exact_ copies. I used `Labels` in the Neo4j version as it's what you would normally do. To explain, this query in CosmosDB:

```
g.V('thomas').addE('knows').to(g.V('mary'))
```

I've translated as:

```
MATCH (thomas:Person {id: 'thomas'}), (mary:Person {id: 'mary'}) CREATE (thomas)-[:KNOWS]->(mary)
```

_But_ - it's actually closer to this:

```
MATCH (thomas {id: 'thomas'}), (mary {id: 'mary'}) CREATE (thomas)-[:KNOWS]->(mary)
```

With that - you get no index support - not really an issue with 4 Nodes. I think this is because with Cosmos you are querying _only_ Person 'vertices'?! So perhaps the `:Person` version is closer.

I've also tried to copy how they do things as close as possible, so for example with the 'Traverse 2x' query, Cosmos has:

```
g.V('thomas').out('knows').hasLabel('person').out('knows').hasLabel('person')
```

And I've put:

```
MATCH (:Person {id: 'thomas'})-[:KNOWS]->(:Person)-[:KNOWS]->(knownKnows:Person) RETURN knownKnows
```

But this could/would be:

```
MATCH (:Person {id: 'thomas'})-[:KNOWS*2]->(knownKnows:Person) RETURN knownKnows
```

I've also used `CREATE` for the Neo4j code, I would actually use `MERGE` if I were doing it properly.

Lastly - I have no idea why 'Thomas' is created with an age of 44, and then gets that updated to 44 later. I assume _reasons_.