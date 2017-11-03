# AppSettings 

Settings need to be registered in ConfigureServices.  P7 does a sweep for any class that inherits from ConfigureServicesRegistrant, mimicking how we sweep for all AutoFac modules.

```
public IServiceProvider ConfigureServices(IServiceCollection services)
{
...
      services.AddAllConfigureServicesRegistrants(Configuration);
      services.AddDependenciesUsingAutofacModules();
...
}
```

## The settings json
```
{
  "graphQLAuthentication": {

    "mutation": {
      "optOut": [ "ted", "bob" ]
    },
    "query": {
      "optOut": [ "resource", "blog" ]
       
      }
    }
  }
}
```

## Preparing the settings for DI
```
public class MutationConfig
{
    public List<string> OptOut { get; set; }
}
public class QueryConfig
{
    public List<string> OptOut { get; set; }
}
public class GraphQLAuthenticationConfig
{
    public const string WellKnown_SectionName = "graphQLAuthentication";
    public MutationConfig Mutation { get; set; }
    public QueryConfig Query { get; set; }
}

public class MyConfigureServicesRegistrant : ConfigureServicesRegistrant
{
    public override void OnConfigureServices(IServiceCollection services)
    {
        services.Configure<GraphQLAuthenticationConfig>(Configuration.GetSection(GraphQLAuthenticationConfig.WellKnown_SectionName));

    }

    public MyConfigureServicesRegistrant(IConfiguration configuration) : base(configuration)
    {
    }
}
    
```

## Using the settings
```
public class AppSettingsGraphQLPermissionsStore : IPermissionsStore
{
    private IOptions<GraphQLAuthenticationConfig> _settings;
    public AppSettingsGraphQLPermissionsStore(IOptions<GraphQLAuthenticationConfig> settings)
    {
        _settings = settings;
    }


    public IEnumerable<string> GetPermissions(OperationType operationType, string field)
    {
        var dataSource = (operationType == OperationType.Query ? _settings.Value.Query.OptOut : _settings.Value.Mutation.OptOut);
        var query = from item in dataSource
            where string.Compare(item,field,CultureInfo.CurrentCulture,CompareOptions.IgnoreCase) == 0
            select item;
        List<string> permissions = new List<string>();
        if (!query.Any())
        {
            permissions.Add("x-graphql-auth");
        }
        return permissions;
    }
}
```

