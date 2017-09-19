using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using GraphQL.Language.AST;

namespace P7.GraphQLCore.Stores
{
    public class InMemoryGraphQLFieldAuthority : IGraphQLFieldAuthority
    {
        private List<GraphQlFieldAuthorityRecord> _listGraphQlFieldAuthorityRecords;

        private List<GraphQlFieldAuthorityRecord> GraphQLFieldAuthorityRecords
        {
            get
            {
                return _listGraphQlFieldAuthorityRecords ??
                       (_listGraphQlFieldAuthorityRecords = new List<GraphQlFieldAuthorityRecord>());
            }
        }
        public async Task<IEnumerable<Claim>> FetchRequiredClaimsAsync(OperationType operationType, string fieldPath)
        {
            var query = from item in GraphQLFieldAuthorityRecords
                where item.OperationType == operationType
                select item;
            GraphQlFieldAuthorityRecord record;
            return !query.Any() ? null : query.FirstOrDefault().Claims;
        }

        public async Task AddClaimsAsync(OperationType operationType, string fieldPath, List<Claim> claims)
        {
            var query = from item in GraphQLFieldAuthorityRecords
                where item.OperationType == operationType
                select item;
            GraphQlFieldAuthorityRecord record;
            if (claims == null)
            {
                claims = new List<Claim>();
            }
            if (!query.Any())
            {
                record = new GraphQlFieldAuthorityRecord()
                {
                    OperationType = operationType,
                    FieldPath = fieldPath,
                    Claims = claims
                };
                GraphQLFieldAuthorityRecords.Add(record);
            }
            else
            {
                record = query.FirstOrDefault();
                var result = record.Claims.Union(claims).ToList();
                record.Claims = result;
            }
        }

        public async Task RemoveClaimsAsync(OperationType operationType, string fieldPath, List<Claim> claims)
        {
            var query = from item in GraphQLFieldAuthorityRecords
                        where item.OperationType == operationType
                        select item;
            GraphQlFieldAuthorityRecord record;
            if (claims == null)
            {
                claims = new List<Claim>();
            }
            if (query.Any())
            {
                record = query.FirstOrDefault();
                var result = record.Claims.Except(claims).ToList();
                record.Claims = result;
            }
        }
    }
}