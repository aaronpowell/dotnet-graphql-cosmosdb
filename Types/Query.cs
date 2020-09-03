using DotNet.GraphQL.CosmosDB.Types.Models;
using HotChocolate.Resolvers;
using HotChocolate.Types;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<QuestionModel> GetQuestion(IResolverContext context, string id)
        {
            var client = (DocumentClient)context.ContextData["client"];

            var collectionUri = UriFactory.CreateDocumentCollectionUri("trivia", "questions");
            var query = client.CreateDocumentQuery<QuestionModel>(collectionUri, new FeedOptions { EnableCrossPartitionQuery = true })
                .Where(q => q.Id == id)
                .AsDocumentQuery();

            while (query.HasMoreResults)
            {
                foreach (var result in await query.ExecuteNextAsync<QuestionModel>())
                {
                    return result;
                }
            }

            throw new ArgumentException("ID does not match a question in the database");
        }
    }

    public class QueryType : ObjectType<Query>
    {
        protected override void Configure(IObjectTypeDescriptor<Query> descriptor)
        {
            descriptor.Field(q => q.GetQuestions(default!))
                .Description("Get all questions in the system")
                .Type<NonNullType<ListType<NonNullType<QuestionType>>>>();

            descriptor.Field(q => q.GetQuestion(default!, default!))
                .Description("Get a question")
                .Type<NonNullType<QuestionType>>();
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
