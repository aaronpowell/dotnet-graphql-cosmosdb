using HotChocolate.AspNetCore;
using HotChocolate.Execution;
using HotChocolate.Language;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents.Client;
using System.Threading;
using System.Threading.Tasks;

namespace HotChocolate.AspNetCore
{
    public interface IGraphQLFunctions
    {
        IAzureFunctionsMiddlewareOptions AzureFunctionsOptions { get; }
        IDocumentCache DocumentCache { get; }
        IDocumentHashProvider DocumentHashProvider { get; }
        IQueryExecutor Executor { get; }
        Task<IActionResult> ExecuteFunctionsQueryAsync(
            HttpContext context,
            DocumentClient client,
            CancellationToken cancellationToken);
    }
}