using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using HotChocolate;
using HotChocolate.AspNetCore;
using DotNet.GraphQL.CosmosDB.Types;

[assembly: FunctionsStartup(typeof(DotNet.GraphQL.CosmosDB.Startup))]

namespace DotNet.GraphQL.CosmosDB
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddSingleton<Query>();

            builder.Services.AddGraphQL(sp =>
                SchemaBuilder.New()
                .AddServices(sp)
                .AddQueryType<QueryType>()
                .Create()
            );
            builder.Services.AddAzureFunctionsGraphQL();
        }
    }
}
