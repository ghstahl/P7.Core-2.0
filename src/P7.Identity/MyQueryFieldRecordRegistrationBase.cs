using System;
using System.Globalization;
using System.Security.Claims;
using GraphQL;
using GraphQL.Language.AST;
using GraphQL.Types;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using P7.Core.Localization;
using P7.GraphQLCore;

namespace P7.Identity
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

           
        }

        public void AddGraphTypeFields(QueryCore queryCore, IPermissionsStore permissionsStore)
        {
            var fieldType = queryCore.FieldAsync<StringGraphType>(name: "accessCode",
                description: null,
                arguments: new QueryArguments(new QueryArgument<IdentityQueryInput> { Name = "input" }),
                resolve: async context =>
                {
                    var userContext = context.UserContext.As<GraphQLUserContext>();

                    var input = context.GetArgument<AccessCodeQueryHandle>("input");


                    return input;
                },
                deprecationReason: null);
            var permissions = permissionsStore.GetPermissions(OperationType.Query, "accessCode");
            foreach (var permission in permissions)
            {
                fieldType.AddPermission(permission);
            }
            
        }
    }
}