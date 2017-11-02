using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQL.Types;

namespace P7.GraphQLCore
{
    public class QueryFieldRecordRegistrationStore : IQueryFieldRecordRegistrationStore
    {
        private IEnumerable<IQueryFieldRecordRegistration> _fieldRecordRegistrations;
        private IPermissionsStore _permissionsStore;
        public QueryFieldRecordRegistrationStore(IEnumerable<IQueryFieldRecordRegistration> fieldRecordRegistrations,
            IPermissionsStore permissionsStore)
        {
            _fieldRecordRegistrations = fieldRecordRegistrations;
            _permissionsStore = permissionsStore;
        }

        public void AddGraphTypeFields(QueryCore queryCore)
        {
            foreach (var item in _fieldRecordRegistrations)
            {
                item.AddGraphTypeFields(queryCore, _permissionsStore);
            }
        }
    }
}
