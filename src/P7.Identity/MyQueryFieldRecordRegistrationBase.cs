using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Security.Claims;
using GraphQL;
using GraphQL.Language.AST;
using GraphQL.Types;
using IdentityModel.Client;
 
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
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

            Field(x => x.AccessToken)
                .Name("access_token")
                .Description("The access_token, may trigger a refresh on the backend.");

            Field<DynamicType>("oidc", "The oidc - this is temporary and will go away");
        }
    }
    public class AccessCodeDocumentHandle
    {
        public string AccessCode { get; set; }
        public string AccessToken { get; set; }
        public object Oidc { get; set; }
    }

    public class MyQueryFieldRecordRegistrationBase : IQueryFieldRecordRegistration
    {
        private IHttpContextAccessor _httpContextAccessor;
        private IConfiguration _configuration;
        private DiscoveryCache _discoveryCache;
        public MyQueryFieldRecordRegistrationBase(
            IHttpContextAccessor httpContextAccessor,
            IConfiguration configuration,
            DiscoveryCache discoveryCache)
        {
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
            _discoveryCache = discoveryCache;
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

                    var selectionSet = context.FieldAst.SelectionSet.Selections;
 
                    var doc = await _discoveryCache.GetAsync();

                    var tokenEndpoint = doc.TokenEndpoint;
                    var keys = doc.KeySet.Keys;

                    var clientId = _configuration["Norton-ClientId"];
                    var cientSecret = _configuration["Norton-ClientSecret"];
                    var client = new TokenClient(
                        doc.TokenEndpoint,
                        clientId,
                        cientSecret);

                    var response = await client.RequestRefreshTokenAsync(oidc["refresh_token"]);
                    var token = response.AccessToken;

                    // TODO get new refresh token if stale.

 
                    var result = new AccessCodeDocumentHandle
                    {
                        Oidc = oidc,
                        AccessToken = "hi",
                        AccessCode = "blah"
                    };
                    return result;
//                    return input;
                },
                deprecationReason: null); 
        }
    }
}