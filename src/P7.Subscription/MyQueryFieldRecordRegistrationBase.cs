using System;
using GraphQL;
using GraphQL.Types;
using Microsoft.AspNetCore.Http;
using P7.GraphQLCore;
using P7.SimpleDocument.Store;

namespace P7.Subscription
{
    public class MetaDataType : ObjectGraphType<MetaData>
    {
        public MetaDataType()
        {
            Name = "metaData";
            Field(x => x.Category).Description("The Category of the subscription.");
            Field(x => x.Version).Description("The Version of the subscription.");
        }
    }
    public class SubscriptionDocumentType : ObjectGraphType<SubscriptionQueryHandle>
    {
        public SubscriptionDocumentType()
        {
            Name = "subscriptionDocument";
           
            Field(x => x.Id).Description("The Id of the Subscription.");
            Field<MetaDataType>("metaData", "The MetaData of the Subscription.");
            //Field<BlogType>("document", "The blog");
        }
    }
    public class MyQueryFieldRecordRegistrationBase : IQueryFieldRecordRegistration
    {
        private IHttpContextAccessor _httpContextAccessor;

        public MyQueryFieldRecordRegistrationBase(
            IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public void AddGraphTypeFields(QueryCore queryCore)
        {

            var fieldName = "subscription";
            var fieldType = queryCore.FieldAsync<SubscriptionDocumentType>(name: fieldName,
                description: null,
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<SubscriptionQueryInput>> { Name = "input" }),
                resolve: async context =>
                {
                    try
                    {
                        var userContext = context.UserContext.As<GraphQLUserContext>();
                        var input = context.GetArgument<SubscriptionQueryHandle>("input");
                        return input;
                    }
                    catch (Exception e)
                    {

                    }
                    return null;
                    //                    return await Task.Run(() => { return ""; });
                },
                deprecationReason: null);

             
        }
    }
}