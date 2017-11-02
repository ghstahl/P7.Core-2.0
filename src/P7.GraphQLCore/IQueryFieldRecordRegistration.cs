using System.Collections;
using System.Collections.Generic;
using GraphQL.Language.AST;

namespace P7.GraphQLCore
{
    public interface IPermissionsStore
    {
        IEnumerable<string> GetPermissions(OperationType operationType,string field);
    }
    public interface IQueryFieldRecordRegistration
    {
        void AddGraphTypeFields(QueryCore queryCore, IPermissionsStore permissionsStore);
    }
}