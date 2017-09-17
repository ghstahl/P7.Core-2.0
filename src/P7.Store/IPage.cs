using System.Collections;
using System.Collections.Generic;

namespace P7.Store
{
    // copied from Cassandra.Mapping
    // this is a really nice abstraction interface to track page requests
    public interface IPage<T> : ICollection<T>, IEnumerable<T>, IEnumerable
    {
        // Summary:
        //     Returns a token representing the state used to retrieve this results.
        byte[] CurrentPagingState { get; }
        //
        // Summary:
        //     Returns a token representing the state to retrieve the next page of results.
        byte[] PagingState { get; }
    }
}