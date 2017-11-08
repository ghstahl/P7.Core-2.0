using System;
using GraphQL;
using GraphQL.Types;
using Microsoft.AspNetCore.Http;
using P7.GraphQLCore;

namespace P7.Subscription
{
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
                        var result = new SubscriptionDocumentHandle
                        {
                            Value = new SomeData() {SubscriptionQueryHandle = input, Ted = "Well Hellow"},
                            Id = input.Id,
                            MetaData = input.MetaData
                        };
                        return result;
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