using HotChocolate.Types;

namespace DotNet.GraphQL.CosmosDB.Types
{
    public class QueryType : ObjectType<Query>
    {
        protected override void Configure(IObjectTypeDescriptor<Query> descriptor)
        {
            descriptor.Field(q => q.GetQuestions(default!))
                .Description("Get all questions in the system")
                .Type<NonNullType<ListType<NonNullType<QuestionType>>>>();

            descriptor.Field(q => q.GetQuestion(default!, default!))
                .Description("Get a question")
                .Argument("id", d => d.Type<IdType>())
                .Type<NonNullType<QuestionType>>();
        }
    }
}
