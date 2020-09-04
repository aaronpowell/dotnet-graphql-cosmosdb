using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate.Execution;
using HotChocolate.Language;
using HotChocolate.Server;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HotChocolate.AspNetCore
{
    public class GraphQLFunctions : IGraphQLFunctions
    {
        public IQueryExecutor Executor { get; }
        public IDocumentCache DocumentCache { get; }
        public IDocumentHashProvider DocumentHashProvider { get; }
        public IAzureFunctionsMiddlewareOptions AzureFunctionsOptions { get; }
        private readonly RequestHelper _requestParser;
        private readonly JsonQueryResultSerializer _jsonQueryResultSerializer;

        public GraphQLFunctions(IQueryExecutor executor, IDocumentCache documentCache,
            IDocumentHashProvider documentHashProvider, IAzureFunctionsMiddlewareOptions azureFunctionsOptions /*JsonQueryResultSerializer jsonQueryResultSerializer*/)
        {
            Executor = executor;
            DocumentCache = documentCache;
            DocumentHashProvider = documentHashProvider;
            AzureFunctionsOptions = azureFunctionsOptions;

            _jsonQueryResultSerializer = new JsonQueryResultSerializer();

            _requestParser = new RequestHelper(
              DocumentCache,
              DocumentHashProvider,
              AzureFunctionsOptions.MaxRequestSize,
              AzureFunctionsOptions.ParserOptions);
        }

        public async Task<IActionResult> ExecuteFunctionsQueryAsync(
            HttpContext httpContext,
            IDictionary<string, object> context,
            CancellationToken cancellationToken)
        {
            using var stream = httpContext.Request.Body;

            var requestQuery = await _requestParser
                .ReadJsonRequestAsync(stream, cancellationToken)
                .ConfigureAwait(false);

            var builder = QueryRequestBuilder.New();

            if (requestQuery.Count > 0)
            {
                var firstQuery = requestQuery[0];

                builder
                    .SetQuery(firstQuery.Query)
                    .SetOperation(firstQuery.OperationName)
                    .SetQueryName(firstQuery.QueryName);

                foreach (var item in context)
                {
                    builder.AddProperty(item.Key, item.Value);
                }

                if (firstQuery.Variables != null
                    && firstQuery.Variables.Count > 0)
                {
                    builder.SetVariableValues(firstQuery.Variables);
                }
            }

            var result = await Executor.ExecuteAsync(builder.Create());
            await _jsonQueryResultSerializer.SerializeAsync((IReadOnlyQueryResult)result, httpContext.Response.Body);

            return new EmptyResult();
        }
    }
}