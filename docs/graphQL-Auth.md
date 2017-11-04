# GraphQL Authorization

P7 implements a single Validator that requires an outside authority to tell it that a path of a query is ok.
```
public interface IGraphQLFieldAuthority
{
    Task<IEnumerable<Claim>> FetchRequiredClaimsAsync(OperationType operationType, string fieldPath);
    Task AddClaimsAsync(OperationType operationType, string fieldPath, List<Claim> claims);
    Task RemoveClaimsAsync(OperationType operationType, string fieldPath, List<Claim> claims);
}
```

The above interface needs to be implemented and registered in the DI. 

TODO: Remove the Add/Remove as the engine doesn't need those methods.

The validator is going to request all the claims that are needed for a given path.
i.e.
```
graphQLFieldAuthority.FetchRequiredClaimsAsync(OperationType.Query,"/accessCode");
graphQLFieldAuthority.FetchRequiredClaimsAsync(OperationType.Query,"/blog/tenantId");

```

Its up to you to answer those questions.  
There is a simple InMemory implemenation you can look at;
```
public class InMemoryGraphQLFieldAuthority : IGraphQLFieldAuthority
{
  ...
}
```
I populate some test data as follows;
```
private async Task LoadGraphQLAuthority()
{
    var graphQLFieldAuthority = P7.Core.Global.ServiceProvider.GetServices<IGraphQLFieldAuthority>().FirstOrDefault();

    await graphQLFieldAuthority.AddClaimsAsync(OperationType.Mutation, "/blog", new List<Claim>()
    {
        new Claim(ClaimTypes.NameIdentifier,""),
        new Claim("client_id","resource-owner-client"),
    });
    await graphQLFieldAuthority.AddClaimsAsync(OperationType.Query, "/accessCode", new List<Claim>()
    {
        new Claim("x-graphql-auth","")
    });
}
```

In my opionion, the real implementation should implement an OptOut model, where you force your users to do something for each root path or you will deny access for anycase.
