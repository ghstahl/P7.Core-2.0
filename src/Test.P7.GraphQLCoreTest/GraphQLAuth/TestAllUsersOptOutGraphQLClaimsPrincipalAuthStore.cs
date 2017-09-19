using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using GraphQL.Language.AST;
using P7.GraphQLCore.Validators;

namespace Test.P7.GraphQLCoreTest.GraphQLAuth
{
    public class TestAllUsersOptOutGraphQLClaimsPrincipalAuthStore : IAllUsersOptOutGraphQLClaimsPrincipalAuthStore
    {
        private static Dictionary<OperationType, Dictionary<string, List<string>>> _individualUsersOptOut;

        private static Dictionary<OperationType, Dictionary<string, List<string>>> IndividualUsersOptOut
        {
            get
            {
                return _individualUsersOptOut ?? (_individualUsersOptOut
                           = new Dictionary<OperationType, Dictionary<string, List<string>>>()
                           {
                               {
                                   OperationType.Query,
                                   new Dictionary<string, List<string>>()
                                   {
                                       {"droids", new List<string>() {"herb"}}
                                   }
                               }
                           });
            }
        }

        public bool Contains(ClaimsPrincipal claimsPrincipal, OperationType operationType, string fieldName)
        {
            if (!IndividualUsersOptOut.ContainsKey(operationType))
                return false;

            var individualMap = IndividualUsersOptOut[operationType];
            if (!individualMap.ContainsKey(fieldName))
                return false;

            var fieldList = individualMap[fieldName];

            var q = from f in fieldList
                where f == fieldName
                select f;
            bool result = q.Any();
            return result;
        }
    }
}