using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Claims;
using GraphQL;
using GraphQL.Language.AST;
using GraphQL.Types;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using P7.Core.Localization;
using P7.GraphQLCore;
using P7.GraphQLCore.Types;
using P7.Core.Utils;
namespace P7.Identity
{
    public class AccessCodeDocumentType : ObjectGraphType<AccessCodeDocumentHandle>
    {
        public AccessCodeDocumentType()
        {
            Name = "accessCodeDocument";
 
            Field(x => x.AccessCode)
                .Name("access_code")
                .Description("The access_code.");
            Field<DynamicType>("oidc", "The oidc - this is temporary and will go away");
        }
    }
    public class AccessCodeDocumentHandle
    {
        public string AccessCode { get; set; }
        public object Oidc { get; set; }
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
            var fieldType = queryCore.FieldAsync<AccessCodeDocumentType>(name: "identity",
                description: null,
                arguments: new QueryArguments(new QueryArgument<IdentityQueryInput> { Name = "input" }),
                resolve: async context =>
                {
                    var userContext = context.UserContext.As<GraphQLUserContext>();

                    var oidc = _httpContextAccessor.HttpContext.Session.GetObject<Dictionary<string, string>>(".oidc");

                    var input = context.GetArgument<AccessCodeQueryHandle>("input");

                    var result = new AccessCodeDocumentHandle
                    {
                        Oidc = oidc,
                        AccessCode = "blah"
                    };
                    return result;
//                    return input;
                },
                deprecationReason: null); 
        }
    }
}