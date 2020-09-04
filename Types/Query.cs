using DotNet.GraphQL.CosmosDB.Models;
using HotChocolate.Resolvers;
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
            var sql = new SqlQuerySpec("SELECT * FROM c WHERE c.id = @id");
            sql.Parameters.Add(new SqlParameter("@id", id));
            var query = client.CreateDocumentQuery<QuestionModel>(collectionUri, sql, new FeedOptions { EnableCrossPartitionQuery = true })
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
}
