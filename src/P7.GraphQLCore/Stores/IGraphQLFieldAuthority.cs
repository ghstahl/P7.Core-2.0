using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using GraphQL.Language.AST;

namespace P7.GraphQLCore.Stores
{
    public interface IGraphQLFieldAuthority
    {
        Task<IEnumerable<Claim>> FetchRequiredClaimsAsync(OperationType operationType, string fieldPath);
        Task AddClaimsAsync(OperationType operationType, string fieldPath, List<Claim> claims);
        Task RemoveClaimsAsync(OperationType operationType, string fieldPath, List<Claim> claims);
    }
}
