using DotNet.GraphQL.CosmosDB.Models;
using HotChocolate.Types;

namespace DotNet.GraphQL.CosmosDB.Types
{
    public class QuestionType : ObjectType<QuestionModel>
    {
        protected override void Configure(IObjectTypeDescriptor<QuestionModel> descriptor)
        {
            descriptor.Field(q => q.Id)
                .Type<IdType>();
        }
    }
}
