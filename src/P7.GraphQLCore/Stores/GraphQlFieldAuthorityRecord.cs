using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using GraphQL.Language.AST;

namespace P7.GraphQLCore.Stores
{
    class GraphQlFieldAuthorityRecord
    {
        public OperationType OperationType { get; set; }
        public string FieldPath { get; set; }
        public List<Claim> Claims { get; set; }

    }
}
