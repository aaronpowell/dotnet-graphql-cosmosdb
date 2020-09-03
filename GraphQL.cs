using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using HotChocolate.AspNetCore;
using System.Threading;
using Microsoft.Azure.Documents.Client;

namespace DotNet.GraphQL.CosmosDB
{
    public class GraphQL
    {
        private readonly IGraphQLFunctions _graphQLFunctions;

        public GraphQL(IGraphQLFunctions graphQLFunctions)
        {
            _graphQLFunctions = graphQLFunctions;
        }

        [FunctionName("graphql")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log,
            [CosmosDB(
                databaseName: "trivia",
                collectionName: "questions",
                ConnectionStringSetting = "CosmosDBConnection")] DocumentClient client,
            CancellationToken cancellationToken)
        {
            return await _graphQLFunctions.ExecuteFunctionsQueryAsync(
                req.HttpContext,
                client,
                cancellationToken);
        }
    }
}
