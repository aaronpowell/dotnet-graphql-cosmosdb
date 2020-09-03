using DotNet.GraphQL.CosmosDB.Types.Models;
using HotChocolate.Resolvers;
using HotChocolate.Types;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DotNet.GraphQL.CosmosDB.Types
{
    public class Query
    {
        public async Task<IEnumerable<QuestionModel>> GetQuestions(IResolverContext context)
        {
            var client = (DocumentClient)context.ContextData["client"];

            var collectionUri = UriFactory.CreateDocumentCollectionUri("trivia", "questions");
            var query = client.CreateDocumentQuery<QuestionModel>(collectionUri)
                .AsDocumentQuery();

            var quizzes = new List<QuestionModel>();

            while (query.HasMoreResults)
            {
                foreach (var result in await query.ExecuteNextAsync<QuestionModel>())
                {
                    quizzes.Add(result);
                }
            }

            return quizzes;
        }
    }

    public class QueryType : ObjectType<Query>
    {
        protected override void Configure(IObjectTypeDescriptor<Query> descriptor)
        {
            descriptor.Field(q => q.GetQuestions(default!))
                .Description("Get all questions in the system")
                .Type<NonNullType<ListType<NonNullType<QuestionType>>>>();
        }
    }

    public class QuestionType : ObjectType<QuestionModel>
    {
        protected override void Configure(IObjectTypeDescriptor<QuestionModel> descriptor)
        {
            descriptor.Field(q => q.Id)
                .Type<IdType>();
        }
    }
}
